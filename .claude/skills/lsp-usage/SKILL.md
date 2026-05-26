---
name: lsp-usage
description: >
  使用 LSP（Language Server Protocol）工具進行程式碼智能分析時適用此 skill。
  當 VS Code Claude 擴充插件的 agent 需要分析程式碼結構、型別、參照或呼叫關係時，
  務必使用此 skill。觸發情境包括：分析類別依賴（DI）、查詢介面實作、追蹤方法呼叫鏈、
  查找符號定義或所有使用位置、確認欄位型別。只要任務涉及 hover、goToDefinition、
  findReferences、documentSymbol、goToImplementation、prepareCallHierarchy、
  incomingCalls、outgoingCalls 等 LSP 操作，就應使用此 skill。
---

# LSP Usage Skill

使用 LSP 工具進行程式碼智能分析時，依照以下規則精確操作。

## 字元偏移量規則：1-based，必須精確

`hover` 與 `goToDefinition` 的游標必須落在**符號 token 上**。落在空白或關鍵字會得不到任何結果。

**呼叫 hover 前，必須先讀取目標行，再手動計算偏移量。**

範例 — C# 欄位宣告：
```
	private readonly ITcsHelper _tcsHelper;
^                   ^           ^
char=1(tab)        char=19     char=30
                   (型別名稱)  (欄位名稱)
```
- `character=10` → 落在 `readonly` → **無輸出**
- `character=19` → 落在 `ITcsHelper` → 回傳型別定義
- `character=30` → 落在 `_tcsHelper` → 回傳欄位型別簽章

偏移量計算公式：從位置 1 開始逐字元計數，tab 算 1 個字元。

## 操作選擇指南

| 目標 | 操作 | 備註 |
|---|---|---|
| 概覽檔案的 classes/methods/fields | `documentSymbol` | 傳入 `line=1, character=1`；位置參數被忽略 |
| 查詢欄位或變數的型別 | `hover` 在**型別名稱** token 上 | 先讀取該行以取得正確字元偏移量 |
| 找到介面/類別的定義位置 | `goToDefinition` 在任意使用該符號的地方 | |
| 找出所有使用某符號的檔案 | `findReferences` 在定義處 | 比 Grep 更精確（排除註解/字串） |
| 找出介面的具體實作類別 | `goToImplementation` 在介面名稱上 | |
| 誰呼叫了某方法 | `incomingCalls`，先在方法名稱上執行 `prepareCallHierarchy` | |
| 某方法呼叫了什麼 | `outgoingCalls`，先在方法名稱上執行 `prepareCallHierarchy` | |

## 何時改用 Grep / Read

- 搜尋哪些檔案含有某字串 → `Grep`（較快）
- 讀取已知檔案的特定段落 → `Read` 搭配 `offset` + `limit`
- `hover` 需要讀取多行才能解析型別時 → 直接 `Read` 建構子區塊

## 標準工作流程：分析類別的 DI 依賴

```
1. documentSymbol(file, 1, 1)
     → 取得建構子行號

2. Read(file, offset=constructor_line, limit=25)
     → 讀取建構子參數，確認型別名稱與位置

3. hover(file, line=param_line, character=<type_name_start>)
     → 確認介面型別（若步驟 2 已清楚則可省略）

4. goToImplementation(interface_file, line, character)
     → 找到具體實作類別

5. findReferences(interface_file, line, character)
     → 找出所有使用該介面的消費者
```
