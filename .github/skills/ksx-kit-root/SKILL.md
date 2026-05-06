---
name: ksx-kit-root
description: Find the actual ks-kit-sdd workspace root path at runtime and avoid hardcoded absolute paths. Use when a prompt, skill, agent, or command needs the current project root string after the repo has been moved.
argument-hint: What root-path or directory-string check do you need?
---

Use this skill when you need the actual root directory string for ks-kit-sdd and the repository may not be located at a fixed absolute path.

## Goal

Treat the ks-kit-sdd workspace root as the folder that contains `.github/`, `notes/`, `disabled/`, and `note.md`. Do not assume a specific absolute path.

## Preferred rules

- In prompts, skills, agents, and instructions, prefer the phrase `ks-kit-sdd workspace root` over a hardcoded absolute path.
- When commands only need location, start from the current workspace root or one of its subdirectories and use relative paths.
- Resolve an absolute path string only when a tool, script, or user explicitly needs one.

## Fast checks

- Already at the root: run `Get-Location`.
- Somewhere inside the git repo: run `git rev-parse --show-toplevel`.
- Need a normalized absolute string from PowerShell: run `Resolve-Path .`.

## Fallback without git

If git is unavailable, walk upward until you find the directory that contains the expected root markers.

```powershell
$path = Get-Location

while ($true) {
	if ((Test-Path (Join-Path $path ".github")) -and
		(Test-Path (Join-Path $path "notes")) -and
		(Test-Path (Join-Path $path "disabled")) -and
		(Test-Path (Join-Path $path "note.md"))) {
		$path
		break
	}

	$parent = Split-Path $path -Parent
	if ([string]::IsNullOrEmpty($parent) -or $parent -eq $path) {
		throw "ks-kit-sdd workspace root not found."
	}

	$path = $parent
}
```

## Output rules

- If the task only needs scope, report `the ks-kit-sdd workspace root`.
- If the task explicitly needs a string, report the derived path from the current environment.
- If the current directory is outside ks-kit-sdd, stop and say the workspace root could not be derived from the current location.