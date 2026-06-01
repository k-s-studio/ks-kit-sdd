---
name: "E2E: User Flow Discovery"
description: Phase 1 - Scan project specs and browse the app to generate a user flow checklist in Overview.md
category: E2E Testing
tags: [e2e, testing, phase1]
---

Phase 1 of the E2E testing workflow: **user flow discovery**.

Run this **inline in the current conversation** — do not delegate to a subagent. This phase is interactive: the user reviews and adjusts the flow list, so the artifacts must stay visible here. First read `.claude/skills/e2e-testing/SKILL.md` and follow its directory conventions, Testnote.md guidance, and behavior rules.

**Input**: No arguments required.

If the project directory and reports directory are not known from conversation context or Testnote.md, ask the user for:
- Project directory (source code and specs)
- Reports directory (where Overview.md and test reports live)

**Steps**
1. Read Testnote.md to restore prior context.
2. Scan the project directory — specs, OpenAPI docs, source — to understand the system's purpose, roles, and features.
3. Use Playwright to log in as an administrator and browse the application: all pages, sidebar items, modals, forms, navigation paths.
4. Write a comprehensive, breadth-first list under `## 使用者流程測試清單` in Overview.md.
5. Update Testnote.md with key findings (admin credentials format, navigation structure, role types, important endpoints).
6. Do **not** create unit folders, report.md files, or design steps in this phase.

**If Phase 1 was already completed**: re-scan and decide whether to append new flows or repair existing content; clearly note what changed.

**Deliverable**: updated Overview.md with a thorough `## 使用者流程測試清單` section.
