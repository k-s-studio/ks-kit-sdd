---
name: "E2E: Design Test Steps"
description: Phase 3 - Design test steps for a specific unit and create its report.md (no execution)
category: E2E Testing
tags: [e2e, testing, phase3]
---

Phase 3 of the E2E testing workflow: **test step design** for a specific unit.

Run this **inline in the current conversation** — do not delegate to a subagent. Designing steps is collaborative: you present the draft report.md and the user adjusts it before any execution. First read `.claude/skills/e2e-testing/SKILL.md` and follow the report.md format defined there exactly.

**Input**: Unit ID is required (e.g., `/e2e:design a-2`). If omitted, ask the user which unit to design.

If the reports directory is not known from conversation context or Testnote.md, ask the user.

**Steps**
1. Read Testnote.md and the unit's entry in Overview.md.
2. Create the unit folder inside the reports directory (e.g., `reports/a-2/`).
3. Create `report.md` following the **exact format** in the skill: metadata table, `## 參數設定` with placeholder comments, `## 單元` steps table (sub-items `a-2-1`, `a-2-2`, …, with 動作 / 預期結果 filled and 測試結果 left blank), and full `## 步驟記錄` with `<!-- screenshot -->` markers.
4. Identify all parameters needed (credentials, URLs, data values) and list them in `## 參數設定`.
5. Do **not** execute the test. Present report.md to the user for review and adjustment.

Note: design and run commands for different units may be interleaved in any order — act only on the specified unit.
