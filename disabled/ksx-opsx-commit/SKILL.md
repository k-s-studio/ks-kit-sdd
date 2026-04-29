---
name: ksx-opsx-commit
description: Commit changes to the repository with a message. Use when an openspec change has been archived.
---

After user called the `/opsx-archive` command, and the spec have been fully archived and synced. Now you can use this skill to commit the changes to the repository with a message.

**steps**
1. **Make sure you are on the correct branch**
    make sure you are on a branch which has name starting with "feat-" or "fix-". make sure you are **not** on the 'Develop' or 'master' branch. You can use `git branch` command to check the current branch.
2. **Stage the relative changes**
    stage the files which are changed by the openspec implementation just been archived including project files and openspec files. you can use `git add` command to stage the changes.
3. **Commit the changes with a message**
    commit the staged changes with a message. the first line of the commit message should be the title of the change, starting with the type of the change ('feat:' or 'fix:') and followed by a bullet point list of what has changed. Title and bullet points should be written in traditional Chinese but keep class and method names in English.
    *example*
    ```
    feat: 用戶登入API
    - 實作`GET api/portal/user/UserPage/GetUserData`
    - 移除User資料表欄位`UserName`
    ```
    ```
    fix: 修正用戶頁 用戶名稱抓不到資料
    - 增加回傳`UserName`欄位
    - 更新測試案例
    ```
    you can use `git commit` command to commit the changes.


