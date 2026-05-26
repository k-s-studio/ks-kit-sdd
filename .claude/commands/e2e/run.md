---
name: "E2E: Run Test"
description: Phase 4 - Execute a designed unit test via Playwright, capture screenshots, and record results in report.md
category: E2E Testing
tags: [e2e, testing, phase4]
---

Launch Phase 4 of the E2E testing workflow: test execution for a specific unit.

**Input**: Unit ID is required (e.g., `/e2e:run a-2`). If omitted, ask the user which unit to run.

**Steps**

Invoke the `e2e-test-collaborator` agent with the following prompt, substituting the unit ID:

```
/e2e:run <unit-id>

Determine the reports directory from conversation context or Testnote.md.
If not available, ask the user.
```

The agent will:
1. Read Testnote.md and the unit's `report.md`
2. Verify all parameters in `## 參數設定` are filled in — stop and ask if any are missing
3. Execute each step in order via Playwright
4. Save screenshots to the unit folder and replace `<!-- screenshot -->` markers with image references
5. Record `☑️` / `✗` in the 測試結果 column after each step
6. Stop and ask the user if a step fails or is blocked
7. Update Overview.md to mark 完成測試 and 測試通過 when all steps pass

Note: design and run commands for different units may be interleaved in any order.
