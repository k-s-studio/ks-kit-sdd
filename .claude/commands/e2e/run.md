---
name: "E2E: Run Test"
description: Phase 4 - Execute a designed unit test via Playwright (in an isolated subagent), capture screenshots, and record results in report.md
category: E2E Testing
tags: [e2e, testing, phase4]
---

Phase 4 of the E2E testing workflow: **test execution** for a specific unit.

Unlike Phases 1–3, this phase **delegates to the `e2e-test-runner` subagent**. Execution drives Playwright through many navigations and screenshots — that browser noise (DOM dumps, screenshot tool results) would flood the main conversation. Isolating it in a subagent keeps this context clean; the subagent reports back only the outcome.

**Input**: Unit ID is required (e.g., `/e2e:run a-2`). If omitted, ask the user which unit to run.

**Steps**

Invoke the `e2e-test-runner` agent (via the Agent tool, `subagent_type: "e2e-test-runner"`) with this prompt, substituting the unit ID and the reports directory:

```
Execute Phase 4 (run) for unit <unit-id>.

Reports directory: <path — from conversation context or Testnote.md; if unknown, ask the user before invoking>.

Follow .claude/skills/e2e-testing/SKILL.md for conventions and the report.md format.
Verify all ## 參數設定 are filled before starting — if any are missing, stop and report back rather than guessing.
Execute each step via Playwright, save screenshots, fill the 測試結果 column, and update Overview.md on completion.
If a step fails or is blocked, stop and report the blocker so the user can decide continue / retry / abort.
```

Relay the subagent's outcome to the user: pass/fail per step, any blockers, and where the report/screenshots live.

Note: design and run commands for different units may be interleaved in any order — the subagent acts only on the specified unit and must not touch another unit's report.md.
