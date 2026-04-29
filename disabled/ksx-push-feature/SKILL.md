---
name: ksx-push-feature
description: Push a feature or fix branch to the origin.
---
Push a feature or fix branch to the origin.

**steps**

1. **Make sure you are on the correct branch** that you want to push. You can check your current branch with:
    ```bash
    git branch
    ```
    **IMPORTANT**: DO NOT push the 'develop', 'master' branch or any other shared branches. Only push feature or fix branches, the branch name should be in the format of `feat-*` or `fix-*`.

2. Make sure the current branch is up to date with and **rebased** on the develop branch. You can use the `ksx-pull-develop` skill to pull the latest changes from the develop branch and rebase your current branch with the develop branch.

3. **Push the current feat or fix branch with --force to the origin.** It is allowed if some changes are not staged or committed.
    ```bash
    git push origin <current-feat-or-fix-branch> --force-with-lease
    ```
