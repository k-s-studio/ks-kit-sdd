---
name: ksx-happy-path-programming
description: Use when generating happy path programming style code.
---

Generate code in a "happy path programming" style, where the main flow of logic is clear and straightforward, and error handling is done separately without cluttering the main logic.

## Concepts
- 所有 controller 只會回傳成功(200)的結果。而遇到應定義為失敗的結果，內部functions會直接拋出Exception

- 不需要考慮例外處理，這些Exception會在Middleware被全局的Exception Handler捕捉並轉換成失敗的結果

- 使用try-catch的唯一時機: 有些內部失敗不需要中斷流程，則catch住、以Ilogger紀錄就不再往外拋

### Controller
- 重點是清楚定義輸入輸出、HTTP方法、Route，某些專案會加上操作權限宣告

- 內部邏輯盡量簡單，通常只是把資料轉給 Service，將複雜的業務邏輯放在 Service 裡面

- 只注入 Service，不直接注入 Repository 或 DbContext

- 不使用 try-catch，讓例外自然冒泡到 Middleware 被統一處理

- 預設回傳型別為 Task\<IActionResult>，並且只回傳成功(200)的結果，使用StatusCode()建立回應:
    ```csharp
    return StatusCode(200, result);
    ```
- 查看專案習慣的回傳方法，也可能採用強型別回傳（例如`response-envelope-pattern`）、或是傳統MVC的 `IActionResult` 符合資料。

---
### Service
#### Service 概述
- 扮演業務協調者的角色，負責協調多個 Repository 以及葉節點 Service 來完成業務邏輯

- 只有 Service 可以使用 try-catch

- 不能注入 DbContext，可以注入不同關鍵字的 Repository

- Service 不可以開 transaction，transaction 應該放在 Repository 裡面確保資料操作的原子性

#### Service 分層規則
Service 分為兩層，由「依賴方向」區分而非命名：

- **Orchestrator Service（協調者）**
    - 協調多個 Capability Service 和 Repository 完成業務流程
    - 可以注入：Capability Service、Repository
    - 對應 Controller 的業務動作（通常 1 Controller 對 1 Orchestrator）
    - 例：AuthService, OrderService, ProductService

- **Capability Service（能力提供者 / 葉節點）**
    - 提供單一可重用能力，被多個 Orchestrator 共用
    - 可以注入：Repository、外部 API Client、其他 Infrastructure
    - ❌ 禁止注入其他 Service（避免循環依賴）
    - 例：
        - 內部能力：MediaService, CacheService, AuditLogService
        - 外部整合：LineAuthService, GoogleAuthService, EmailService

**判斷原則**
- 這個 Service 會被 2 個以上其他 Service 使用嗎？→ Capability Service

- 這個 Service 是某個業務流程的進入點嗎？→ Orchestrator Service

- 不確定時 → 先當 Orchestrator，被重用時再降為 Capability
---
### Repository
- 負責與資料庫溝通，取得 Service 需要的資料

- 操作涉及超過一張資料表 (例如一對多、多對多的資料建立) 開 transaction 確保操作的原子性；**transaction 不包在try-catch裡面**，若失敗會throw Exception 阻止Commit，讓資源釋放時自動回滾。

- 不可以注入 Service 以及其他 Repository

- 只有 Repository 可以注入 DbContext
