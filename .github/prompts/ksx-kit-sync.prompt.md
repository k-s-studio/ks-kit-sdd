---
name: ksx-kit-sync
description: Sync local ks-sdd-kit changes to origin/main with an auto-generated short commit message, then stop on any conflict.
---

#agent: ksx-kit-manager

Sync local `ks-sdd-kit` with `origin/main`.

Use the `ksx-kit-sync` skill for the git command flow and stop conditions.

Do this:
- Work only in `c:\Workspace\ks-sdd-kit`.
- Operate on `main` only. If the current branch is not `main`, stop and report.
- Stage all local changes.
- Generate one short commit message from the actual staged diff, then commit only if there is something to commit.
- Pull the latest `origin/main`, then push local `main` to `origin/main`.
- If `pull` or `push` hits a conflict, stop immediately and tell the user to resolve it manually.
- Report the branch, the commit message used, and the final sync result.

