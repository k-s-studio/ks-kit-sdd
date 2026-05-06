---
name: ksx-ready-to-merge
description: "Make the current branch mergeable by removing branch-only EF Core migrations, restoring ModelSnapshot, and rolling the local DB back to the pre-branch migration."
argument-hint: "Optional target branch, for example: origin/develop"
#agent: ksx
---

Make the current branch mergeable.

Create a blocker task list first and work through it one by one. Start with these blockers, then append any new blockers you discover:
- `*ModelSnapshot.cs` changed on this branch
- EF Core migrations were added on this branch
- The local database is on a migration newer than the last migration that existed before this branch diverged

Rules:
- Use commands, not manual edits, for migration cleanup.
- Never manually edit files under `Database/SqlServerMigrations/Migrations/`.
- Restore snapshot files from the branch-base commit, not from the current target-branch tip.
- Show the commands you run.

Use PowerShell commands unless the user explicitly asks for another shell.

Flow:

1. Determine the comparison branch.
   - If the user provided a branch, use it.
   - Otherwise prefer the first existing branch from: `origin/develop`, `develop`, `origin/master`, `master`.

   ```powershell
   $targetCandidates = @($inputBranch, 'origin/develop', 'develop', 'origin/master', 'master') | Where-Object { $_ }
   $target = $null
   foreach ($candidate in $targetCandidates) {
       git rev-parse --verify $candidate *> $null
       if ($LASTEXITCODE -eq 0) {
           $target = $candidate
           break
       }
   }
   if (-not $target) {
       throw 'Cannot determine the target branch. Provide it explicitly.'
   }
   $mergeBase = git merge-base HEAD $target
   ```

2. Inspect git state and migration-related diffs.

   ```powershell
   git status --short
   git diff --name-status $mergeBase -- Database/SqlServerMigrations/Migrations
   ```

3. Determine the last migration that already existed at the branch base.

   ```powershell
   $baseMigrationFile = git ls-tree -r --name-only $mergeBase Database/SqlServerMigrations/Migrations |
       Where-Object { $_ -match 'Database/SqlServerMigrations/Migrations/\d{14}_.+\.cs$' -and $_ -notmatch '\.Designer\.cs$' } |
       Split-Path -Leaf |
       Sort-Object |
       Select-Object -Last 1

   $baseMigrationName = if ($baseMigrationFile) {
       [System.IO.Path]::GetFileNameWithoutExtension($baseMigrationFile)
   } else {
       '0'
   }
   ```

4. Inspect the current DB migration state.

   ```powershell
   dotnet ef migrations list --context FubonEPodDbContext -p Database/SqlServerMigrations/Database.SqlServerMigrations.csproj
   ```

   Treat the last migration line without `(Pending)` as the current DB migration.

5. If the DB is ahead of `$baseMigrationName`, roll it back to the branch-base migration.

   ```powershell
   dotnet ef database update $baseMigrationName --context FubonEPodDbContext -p Database/SqlServerMigrations/Database.SqlServerMigrations.csproj
   ```

6. Remove every migration that exists only on the current branch. Compute the current-on-disk migrations minus the migrations that existed at `$mergeBase`, then remove from newest to oldest.

   ```powershell
   $baseMigrationFiles = git ls-tree -r --name-only $mergeBase Database/SqlServerMigrations/Migrations |
       Where-Object { $_ -match 'Database/SqlServerMigrations/Migrations/\d{14}_.+\.cs$' -and $_ -notmatch '\.Designer\.cs$' } |
       Split-Path -Leaf

   $currentMigrationFiles = Get-ChildItem Database/SqlServerMigrations/Migrations -File |
       Where-Object { $_.Name -match '^\d{14}_.+\.cs$' -and $_.Name -notmatch '\.Designer\.cs$' } |
       Select-Object -ExpandProperty Name

   $branchMigrationNames = $currentMigrationFiles |
       Where-Object { $_ -notin $baseMigrationFiles } |
       ForEach-Object { [System.IO.Path]::GetFileNameWithoutExtension($_) } |
       Sort-Object -Descending

   foreach ($migration in $branchMigrationNames) {
       dotnet ef migrations remove --context FubonEPodDbContext -p Database/SqlServerMigrations/Database.SqlServerMigrations.csproj
   }
   ```

   Stop and report if the number of removed migrations does not match the computed blocker list.

7. Discard snapshot changes and restore them to the branch-base state.

   ```powershell
   $changedSnapshots = git diff --name-only $mergeBase -- 'Database/SqlServerMigrations/Migrations/*ModelSnapshot.cs'
   foreach ($snapshot in $changedSnapshots) {
       git restore --source=$mergeBase -- $snapshot
   }
   ```

8. Re-check the blockers and summarize the result.

   ```powershell
   git status --short
   git diff --name-status $mergeBase -- Database/SqlServerMigrations/Migrations
   dotnet ef migrations list --context FubonEPodDbContext -p Database/SqlServerMigrations/Database.SqlServerMigrations.csproj
   ```

Report:
- blocker task list and which items were resolved
- target branch and merge-base commit
- base migration before the branch
- current DB migration before and after rollback
- migrations removed by command
- snapshot files restored from `$mergeBase`
- any remaining blockers