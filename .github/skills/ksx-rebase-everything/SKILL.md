---
name: ksx-rebase-everything
description: Rebase the current feature branch onto the latest target branch with full safety: preserve uncommitted changes, prevent EF Core migration / DbContextModelSnapshot conflicts, regenerate migrations on top, apply to local DB, and run related tests.
---

當使用者要把 feature branch rebase 到最新的 develop（或其他目標分支）時使用本 skill。**不論該分支有沒有改 EF migration，都跑同一套流程**——預防衝突 → rebase → 解必須解的衝突 → 重產 migration → 套用到本地 DB → 跑測試。

## 使用前提（先檢查，不符合就終止）

### 必要環境

- 必須在 git repo 內
- 必須在 feature branch 上（不能在 `develop` / `master` / `main`）
- 必須能 `git fetch` 到 remote

### 工作目錄狀態

容許「有未 commit / 未 staged / untracked 的檔案」。流程會用 `git stash --include-untracked` 暫存，rebase 完成後 `git stash pop` 回來。

**會終止流程的情況**：

- 工作目錄處於 rebase / merge / cherry-pick 進行中（先要求使用者收尾或 `--abort`）
- 有 unmerged paths（衝突未解）
- `git stash --include-untracked` 失敗（例如有 submodule 改動 stash 不下來）—— 告訴使用者具體原因，請他自行處理或同意捨棄

## 流程總覽

```
0. Pre-flight checks
1. 偵測 repo 慣例（startup project、migrations project、DbContext、本地環境名、tests project）
2. Stash WIP（若有）
3. 建立備份分支
4. fetch + 偵測「本分支是否有 migration commits」
5. git rebase origin/<target>
6. 衝突解決：
   - Migration .cs / .Designer.cs：刪掉本分支的舊檔
   - Snapshot：保留 develop 版本
   - Test 專案檔案：兩者皆保留；若出現編譯錯誤則刪除衝突檔，Step 9 前重新生成
   - 其他手寫檔（DbContext、Program.cs、resx 等）：優先兩者皆保留；相悖時停下交給使用者
7. Rebase 完成後：若步驟 4 偵測到本分支有 migration → 重新 `dotnet ef migrations add <Name>`
8. dotnet ef database update（本地環境）
9. 跑相關 tests
10. Stash pop（還原 WIP）
11. 回報結果與下一步建議（push --force-with-lease）
```

## 詳細步驟

### Step 0：Pre-flight checks

```powershell
git rev-parse --is-inside-work-tree                   # 必須是 git repo
git symbolic-ref --short HEAD                         # 取得目前 branch 名
Test-Path .git/MERGE_HEAD, .git/CHERRY_PICK_HEAD, .git/rebase-merge, .git/rebase-apply
git status --porcelain                                 # 看是否有 unmerged (UU/AA/DD/AU/UA/DU/UD)
```

不符合上述「使用前提」 → 立即停止並回報原因，不做任何修改。

### Step 1：偵測 repo 慣例

不要 hardcode；用以下規則偵測，找不到就向使用者確認：

| 項目 | 偵測方式 | FubonEPod 預期值 |
|---|---|---|
| **目標分支** | 預設 `origin/develop`；找不到則用 `origin/main` / `origin/master` | `origin/develop` |
| **Solution** | repo root 的 `*.sln` | `FubonEPod.sln` |
| **Migrations project** | `**/Database/**/*Migrations*.csproj`，或 `Migrations/` 資料夾所在的 csproj | `Database/SqlServerMigrations/Database.SqlServerMigrations.csproj` |
| **Migrations folder** | Migrations project 裡的 `Migrations/` 子資料夾 | `Database/SqlServerMigrations/Migrations/` |
| **DbContext name** | 在 Migrations folder 找 `*ModelSnapshot.cs`，從檔名或 `[DbContext(typeof(XxxDbContext))]` 取 | `FubonEPodDbContext` |
| **Startup project** | 同時有 `appsettings.LocalDocker.json` 與 `Program.cs`，且引用 Migrations project；多個則挑與本分支 changes 最相關的 | `AdminAngular/AdminAngular.csproj` |
| **本地測試環境名** | startup project 內 `appsettings.{ENV}.json` 中扣掉 `Development`、`Staging`、`Production`、`Test` 之外的；通常是 `LocalDocker` | `LocalDocker` |
| **Tests project** | repo 內 `Tests*.csproj` / `*.Tests.csproj` | `Tests/Tests.csproj` |

> 偵測到多個候選且難以判斷時，先列出來請使用者選一個，再繼續。

### Step 2：Stash WIP

```powershell
$dirty = git status --porcelain
if ($dirty) {
    git stash push --include-untracked --message "ksx-rebase-everything-WIP-$(Get-Date -Format 'yyyyMMddHHmmss')"
}
```

stash 失敗就終止並告知使用者。記得**最後**要 pop。

### Step 3：建立備份分支

```powershell
$branch = git symbolic-ref --short HEAD
$ts = Get-Date -Format 'yyyyMMddHHmmss'
git branch "backup/$branch-pre-rebase-$ts"
```

讓使用者萬一不滿意可以 `git reset --hard backup/...` 回來。

### Step 4：fetch + 偵測 migration 風險

```powershell
git fetch origin
$target = "origin/develop"   # 或偵測結果
$mergeBase = git merge-base HEAD $target

# 本分支獨有的 commits 是否動到 migrations 資料夾
$branchMigrationFiles = git diff --name-only "$mergeBase..HEAD" -- "$migrationsFolder"

# 目標分支是否動過 migrations 資料夾（決定衝突可能性，不影響流程，但要回報）
$targetMigrationFiles = git diff --name-only "$mergeBase..$target" -- "$migrationsFolder"
```

把 `$branchMigrationFiles` 中**屬於本分支新增的 migration 檔名（不含 Snapshot）** 記下來，用於 Step 7 重產時的命名。例如：
- 找出 pattern `Migrations/<timestamp>_<Name>.cs` 對應的 `<Name>` 列表
- 通常一個 feature branch 只有 1 個，多個就以最新時間戳的為主名，其他需要使用者確認

### Step 5：執行 rebase

```powershell
git rebase $target
```

- 沒衝突 → 跳到 Step 7
- 有衝突 → Step 6

### Step 6：解衝突

依檔案類型分類處理：

#### 6a. Migration source 檔案 (`Migrations/<timestamp>_*.cs` / `*.Designer.cs`)

**規則：本分支的 migration 一律刪掉（Step 7 會重產）**

```powershell
# 列出所有衝突的 migration 檔（排除 Snapshot）
$conflictMigrations = git diff --name-only --diff-filter=U -- "$migrationsFolder" |
    Where-Object { $_ -notlike "*ModelSnapshot.cs" }

foreach ($f in $conflictMigrations) {
    git rm -f $f
}
```

#### 6b. `FubonEPodDbContextModelSnapshot.cs`

**規則：保留目標分支版本（rebase 期間 `--ours` 與 `--theirs` 是反的；正確做法是用 origin/<target> 的版本）**

```powershell
git checkout $target -- "$migrationsFolder/FubonEPodDbContextModelSnapshot.cs"
git add "$migrationsFolder/FubonEPodDbContextModelSnapshot.cs"
```

> ⚠️ 不要用 `git checkout --ours` 或 `--theirs` 來解 snapshot，rebase 模式下方向會搞錯。直接從 `$target` 取最確定。

#### 6c. Test 專案內的衝突檔案

**規則：一律嘗試兩者皆保留（合併雙方新增的測試內容）。**

- 讀取衝突檔案，把 `<<<<<<<` / `=======` / `>>>>>>>` 區塊兩側都保留下來
- 解完後 `git add <file>`

若合併後出現**編譯錯誤**（例如重複的 class 定義、method 簽名衝突）：
1. 直接刪除該衝突檔案（`git rm -f <file>`）
2. 記錄被刪除的檔案清單，Step 9 跑測試前**先重新生成**這些測試檔（依照原本的測試邏輯重寫）
3. `git add` 重新生成的檔案後繼續

#### 6d. 其他手寫檔案（DbContext.cs、Program.cs、resx、Designer.cs for resx、entity class…）

**規則：優先嘗試兩者皆保留。**

- 讀取衝突檔案，把 `<<<<<<<` / `=======` / `>>>>>>>` 區塊用語意合併
- **DbContext.cs**：兩邊新增的 `DbSet<>` 都要保留（合併兩段 entity）
- **Program.cs**：兩邊新增的 `using` 與 `AddScoped<>` 都要保留
- **`*.resx` / `Localization.Designer.cs`**：兩邊新增的 key 都要保留
- 解完後 `git add <file>`

若兩方變更**相悖**（例如一邊刪除某行、另一邊修改同一行；或一邊移除某個 DbSet、另一邊新增對它的依賴）→ 停下來，把衝突區塊原文列給使用者，請使用者裁示要保留哪邊。**不要猜**。

#### 6e. 確認所有衝突已解

```powershell
git status --porcelain | Where-Object { $_ -match '^(U.|.U|AA|DD)' }
```

無輸出才能繼續：

```powershell
git rebase --continue
```

如果還有後續 commits 也產生衝突 → 回到 Step 6 重複處理。

#### 6f. 偵測到無法自動處理的衝突

例如：
- 兩邊都改同一張 entity 的 fluent config 語意衝突
- DbContext 的 partial 拆檔內互相矛盾的設定

→ 暫停並請使用者裁示要保留哪邊。**不要猜**。

### Step 7：重產 migration（若 Step 4 偵測本分支有 migration）

```powershell
$migName = "<從 Step 4 取得的名稱>"   # 例如 AddMailTemplate
dotnet ef migrations add $migName `
    --context $dbContextName `
    -p $migrationsProject `
    -s $startupProject `
    -o Migrations
```

驗證新檔內容只包含本分支的 schema 變更（不應該含目標分支已存在的表）。如果 diff 出現大量本來就在 develop 的表 → 代表 Step 6b 的 snapshot 沒對齊好，回去重新處理。

### Step 8：套用到本地 DB

```powershell
dotnet ef database update `
    --context $dbContextName `
    -p $migrationsProject `
    -s $startupProject `
    -- --environment $localEnv
```

`$localEnv` 取 Step 1 偵測到的本地環境名（FubonEPod 是 `LocalDocker`）。

**絕對不要**在這個 skill 裡用 `Staging` / `Production` / 任何看起來像線上的環境名。如果偵測不到本地環境名 → 停下來問使用者，不要預設。

如果 update 失敗：
- 連線錯誤 → 提醒使用者啟動本地 DB 容器（FubonEPod 通常是 `DockerServices/` 內的 compose）
- Migration apply 錯誤 → 列出錯誤原文，先停下來別繼續測試

### Step 9：跑測試

若 Step 6c 有**刪除**任何 Test 專案的衝突檔案，必須先重新生成這些測試檔，再跑測試：

```powershell
# （若有被刪的測試檔，依原本測試邏輯重寫並 git add）
git add Tests/
```

```powershell
# 跑與本分支 changes 相關的測試
dotnet test $testsProject --nologo --no-restore
```

可以用 `--filter` 縮小範圍（例如本分支動過 `MailTemplate*` → `--filter "FullyQualifiedName~MailTemplate"`），但全跑也可以。

回報通過 / 失敗數量與失敗的測試名稱。失敗 ≠ rebase 失敗，但要明確告知使用者哪些測試需要修。

### Step 10：Stash pop

```powershell
if ($stashed) {
    git stash pop
}
```

如果 pop 出現衝突：
- 告知使用者哪些檔案有衝突
- **不要**自動解
- 也不要刪 stash entry，保留讓使用者自己處理

### Step 11：commit 重產的 migration（若 Step 7 有跑）

```powershell
git add "$migrationsFolder/"
git commit -m "feat(migration): regenerate $migName onto $target after rebase"
```

### Step 12：回報

回報內容必須包含：

1. ✅ / ❌ 各步驟結果
2. 備份分支名稱（`backup/...`）
3. 本分支與目標分支的最終 commit 落差
4. 重產的 migration 檔名（若有）
5. DB update 是否成功
6. 測試結果摘要
7. **下一步指令建議**：
   ```powershell
   git push --force-with-lease
   ```
   並提醒「rebase 改寫了歷史，必須 force push；用 `--force-with-lease` 不要用 `--force`」

## 失敗復原

如果中途任何步驟出錯且自動處理不了：

```powershell
# 1. 取消 rebase
git rebase --abort

# 2. 還原到備份
git reset --hard "backup/<branch>-pre-rebase-<timestamp>"

# 3. 還原 WIP
git stash pop   # 如果有 stash
```

把這三條指令印給使用者，讓他能一鍵還原。

## 不要做的事

- ❌ 不要在 develop / master / main 上跑這個 skill
- ❌ 不要用任何看起來像線上環境的名字跑 `dotnet ef database update`
- ❌ 不要對 Snapshot 做手動 3-way merge（一定走 Step 6b 從目標分支取）
- ❌ 不要用 `git checkout --ours` / `--theirs` 解 Snapshot（rebase 下方向相反容易搞錯）
- ❌ 不要 `git push --force`（一律用 `--force-with-lease`）
- ❌ 不要在 stash pop 出衝突時自動解（讓使用者決定）
- ❌ 不要在 Step 6 偵測到語意衝突時自己猜（停下來問）

## 為什麼要這樣設計

- **Snapshot 是 generated**：手動 merge 會產生跟最後一個 migration 的 Designer.cs 不一致的狀態，導致 `PendingModelChangesWarning` 或 migration 被誤產。直接從目標分支拿最新版 + 重 add 是唯一可靠做法。
- **Migration 檔有時間戳順序**：本分支 migration 時間戳若早於目標分支新加的，rebase 後即使沒衝突也會被 EF 忽略（`__EFMigrationsHistory` 不會倒回去重跑）。一律重產確保時間戳在最後。
- **Stash 而非要求 clean tree**：使用者習慣同時改多個東西；強制要求 commit 才能 rebase 會干擾他的工作節奏。
- **本地環境而非線上**：rebase 後跑 `database update` 是為了驗證 migration 可執行，**不是**部署。一定要在本地環境。
