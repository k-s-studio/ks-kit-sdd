---
name: ksx-pull-develop
description: Pull the latest changes from the develop branch and, if on a feature branch, rebase the current branch.
---
Pull the latest changes from the develop branch and, if on a feature branch, rebase the current branch.

**steps**

1. **Get the latest develop**

    if you are on the develop branch, pull the latest changes from the develop branch. any conflicts should be resolved manually.
    ```bash
    git pull origin develop
    ```

    and if you are on a feature branch, fetch the latest changes from the develop branch. any conflicts should be resolved manually.
    ```bash
    git fetch origin develop:develop
    ```

2. **Rebase the current branch with develop**

    if you are on the develop branch, you can skip this step.
    if you are on a feature branch, and the the develop brance has been fetched with no conflicts, rebase the current branch with the develop branch. any conflicts should be resolved manually.
    ```bash
    git pull . develop --rebase
    ```
