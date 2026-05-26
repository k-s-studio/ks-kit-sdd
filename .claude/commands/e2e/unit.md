---
name: "E2E: Unit Test Proposal"
description: Phase 2 - Propose organized unit test items based on the user flow checklist in Overview.md
category: E2E Testing
tags: [e2e, testing, phase2]
---

Launch Phase 2 of the E2E testing workflow: unit test proposal.

**Input**: No arguments required. Requires Phase 1 (`/e2e:userflow`) to have been completed first.

**Steps**

Invoke the `e2e-test-collaborator` agent with the following prompt:

```
/e2e:unit

Determine the project directory and reports directory from conversation context or Testnote.md.
If neither is available, ask the user to provide them.
```

The agent will:
1. Read Testnote.md and the `## 使用者流程測試清單` in Overview.md
2. Group flows into logical units (by role or feature)
3. Assign alphanumeric IDs (a-1, a-2, b-1, ...)
4. Write a `## 單元測試項目` table in Overview.md with columns: 編號 | 單元名稱 | 說明
5. Prioritize clarity and organization over quantity
