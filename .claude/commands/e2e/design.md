---
name: "E2E: Design Test Steps"
description: Phase 3 - Design test steps for a specific unit and create its report.md (no execution)
category: E2E Testing
tags: [e2e, testing, phase3]
---

Launch Phase 3 of the E2E testing workflow: test step design for a specific unit.

**Input**: Unit ID is required (e.g., `/e2e:design a-2`). If omitted, ask the user which unit to design.

**Steps**

Invoke the `e2e-test-collaborator` agent with the following prompt, substituting the unit ID:

```
/e2e:design <unit-id>

Determine the reports directory from conversation context or Testnote.md.
If not available, ask the user.
```

The agent will:
1. Read Testnote.md and the unit's entry in Overview.md
2. Create the unit folder inside the reports directory
3. Create `report.md` with the standard format: metadata table, parameters (with placeholders), unit steps table, and full step-by-step records with `<!-- screenshot -->` markers
4. Present the report to the user for review — **do not execute yet**

Note: design and run commands for different units may be interleaved in any order.
