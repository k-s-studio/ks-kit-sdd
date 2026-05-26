---
name: "E2E: User Flow Discovery"
description: Phase 1 - Scan project specs and browse the app to generate a user flow checklist in Overview.md
category: E2E Testing
tags: [e2e, testing, phase1]
---

Launch Phase 1 of the E2E testing workflow: user flow discovery.

**Input**: No arguments required.

**Steps**

Invoke the `e2e-test-collaborator` agent with the following prompt:

```
/e2e:userflow

Determine the project directory and reports directory from conversation context or Testnote.md.
If neither is available, ask the user to provide:
- Project directory (source code and specs)
- Reports directory (where Overview.md and test reports live)
```

The agent will:
1. Read Testnote.md to restore prior context
2. Scan project specs and source to understand features and roles
3. Log in as admin via Playwright and browse all pages
4. Write a comprehensive `## 使用者流程測試清單` section in Overview.md
5. Update Testnote.md with key findings
