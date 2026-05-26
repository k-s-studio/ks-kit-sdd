- skill: ef-dev-integretion
	- 任何時候都不要直接修改snapshot和migration，使用指令操作。
	- 如果agent要執行commit時，不要將snapshot與migration加入變更
- skill: coop-dev-mode
	- 不修改README、instruction, skill 除非使用者明確提出「幫我新增README/instruction/skill」或「幫我修改README/instruction/skill」
	- 分辨當前變更的關鍵字
	- 確認工作分支是來自哪裡
- skill: get-ready-to-merge
	- 將database還原至原本的migration上，然後還原snapshot與migration
- ProjectMemory.md
	- 自由讀取自由修改，所有分支共用一個檔案
	- 針對每個分支記下分支名稱、分支實作關鍵字
	- 記下分支剛建立時的資料庫是在哪個Migration

兩種之間怎麼同步? 是 /agent 能共用，還是用一個 claude agent 專門改寫導入? 未來以哪邊為優先?
所以插件式就沒有 command 資料夾了嗎 那skill和prompt差在哪 怎麼這麼隨便...