---
name: "E2E: Unit Test Proposal"
description: Phase 2 - Propose organized unit test items based on the user flow checklist in Overview.md
category: E2E Testing
tags: [e2e, testing, phase2]
---

Phase 2 of the E2E testing workflow: **unit test proposal**.

Run this **inline in the current conversation** — do not delegate to a subagent. The user reviews and adjusts the proposed units here. First read `.claude/skills/e2e-testing/SKILL.md` and follow its conventions.

**Input**: No arguments required. Requires Phase 1 (`/e2e:userflow`) to have been completed first.

If the project/reports directories are not known from conversation context or Testnote.md, ask the user.

**Steps**
1. Read Testnote.md and the `## 使用者流程測試清單` section in Overview.md.
2. Group user flows into logical units — each unit typically covers one permission role or one system feature.
3. Assign each unit an alphanumeric ID (`a-1`, `a-2`, `b-1`, …) that also serves as its folder name.
4. Write a `## 單元測試項目` table in Overview.md with columns: `編號` | `單元名稱` | `說明`.
5. Prioritize **organizational clarity** over quantity — comprehensive but not redundant.
6. Do **not** create unit folders, report.md files, or design steps in this phase.
7. Update Testnote.md if you discover new context.

**If Phase 2 was already completed**: re-read the current user flow list, compare against existing units, propose additions/modifications, clearly marking what is new or changed.

**Deliverable**: updated Overview.md with a `## 單元測試項目` table.
