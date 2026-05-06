---
name: ksx-kit-sync
description: Sync local ks-kit-sdd changes to origin/main with an auto-generated short commit message, then stop on any conflict.
---

#agent: ksx-kit-manager

Sync local `ks-kit-sdd` with `origin/main`.

Do this:
- Work only in the ks-kit-sdd workspace root.
- If an absolute path string is needed, derive it from the current workspace root instead of assuming a fixed location.
- Operate on `main` only. If the current branch is not `main`, stop and report.
- If the repo is already in merge, rebase, or cherry-pick state, stop and report.
- Stage all local changes.
- Use the available git tooling for the actual add, diff, commit, pull, and push operations instead of hardcoding a shell command script.
- Generate one short commit message from the actual staged diff. Keep it specific and short, for example `Add <feature>`, `Update <feature>`, or `Refine <feature>`.
- Commit only if there is something staged to commit.
- Pull the latest `origin/main`, then push local `main` to `origin/main`.
- Do not create or switch branches.
- Do not run destructive cleanup commands such as `git reset --hard` or `git clean -fd`.
- Do not auto-resolve conflicts.
- If pull or push hits a conflict, pause for manual resolution, or requires user auth/policy input, stop immediately.

When stopping, report:
- current branch
- whether a commit was created
- the commit message used, if any
- the command or operation that stopped
- that the user must resolve it manually before retrying

When finished successfully, report:
- current branch
- whether a commit was created and by whom (git config user.email)
- the commit message used, if any
- whether pull and push both completed