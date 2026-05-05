---
description: sync ksx-sdd-kit from and to remote.
---

Use cmd or powershell, cd to ks-sdd-kit folder, add and sync all changes to remote.

No need to make any new branch or control any version. Just pull and push the **main** branch.

Generate message for commit automatically e.g. "Add new prompt ksx-kit-sync"

Use these commands:
```bash
git add .
git commit -m "{commit message}"
```

```bash
git pull origin
git push origin
```