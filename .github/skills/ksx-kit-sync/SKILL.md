---
name: ksx-kit-sync
description: Git command set and stop conditions for syncing the ks-sdd-kit main branch with origin/main.
---

Use this skill when a prompt or user asks to sync `c:\Workspace\ks-sdd-kit` with remote.

## Goal

Stage local changes, derive one short commit message from the actual diff, commit, pull the latest `origin/main`, and push local `main`. Stop on any conflict.

## Required checks

- Current working directory must be `c:\Workspace\ks-sdd-kit`.
- Current branch must be `main`.
- If the repo is already in merge, rebase, or cherry-pick state, stop and report.

## Preferred command flow

```powershell
Get-Location
git branch --show-current
git status --short
git add -A
git diff --cached --name-status
git diff --cached --stat
```

Use the staged file list and diff summary to derive a short subject line. Keep it specific and short.

Preferred commit message patterns:
- `Add <feature>`
- `Update <feature>`
- `Refine <feature>`
- `Add <feature> skill and prompt`

Examples:
- `Add ksx-kit-sync skill and prompt`
- `Update ksx-kit-sync prompt`
- `Refine ksx ready-to-merge prompt`

If `git diff --cached --quiet` exits with `0`, there is nothing staged to commit. In that case, skip commit and continue to pull.

```powershell
git commit -m "<short message>"
git pull --rebase origin main
git push origin main
```

## Stop conditions

- `git pull --rebase origin main` reports conflicts or pauses for manual resolution.
- `git push origin main` is rejected and needs manual intervention.
- Authentication or policy checks require user input you cannot complete safely.

When stopping, report:
- current branch
- whether a commit was created
- the commit message used, if any
- the command that stopped
- that the user must resolve the conflict manually before retrying

## Guardrails

- Do not create or switch branches.
- Do not run destructive cleanup commands such as `git reset --hard` or `git clean -fd`.
- Do not auto-resolve conflicts.
- Keep all work scoped to `c:\Workspace\ks-sdd-kit`.