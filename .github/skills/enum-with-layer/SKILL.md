---
name: enum-with-layer
description: Create a C# layered enum-like value object when a flat enum would make guarded, rare, or high-risk choices too easy to select. Use for non-flat enum, layered enum, nested enum options, strongly typed enum, or enum choices grouped behind an explicit namespace-like layer.
argument-hint: Describe the target choices, which choices need a nested layer, and the target file conventions.
---

當使用者需要「不平坦的 enum」時使用本 skill：它仍然要像 enum 一樣輕量、可比較、可被靜態成員列舉，但某些選項必須刻意經過一層巢狀入口，避免呼叫端隨手選到。

## 隱私與泛化規則

- 這個 kit 內的 skill 只能記錄抽象結構，不要寫入來源專案的 class、member、parameter、namespace、錯誤訊息、業務名詞或範例字串。
- 若你是從某個實際 symbol 學到這個模式，只抽取形狀：`readonly struct`、private inner enum、private constructor、top-level static choices、nested static choice group、intent-check property、equality/hash。
- 在 skill 範例中一律使用模糊名稱，例如 `LayeredChoice`、`InnerValue`、`DefaultPath`、`Gated`、`SpecialPath`、`candidate`。
- 在目標專案實作時才使用目標專案自己的命名；不要把目標專案命名回寫到 kit 的 skill、prompt、agent 或 notes。

## 適用情境

- 平坦 enum 會讓危險、稀有、內部或需審核的選項跟普通選項同等容易被選到。
- 需要 API 外觀像 `ChoiceType.Normal` 與 `ChoiceType.Gated.Special`，用呼叫端語法表達「這是刻意選擇」。
- 不想暴露 raw enum 或任意 numeric/string conversion，但仍想保有值型別、預設安全值、比較與 hash。

## 建模流程

1. 先列出所有選項，標出哪些是一般選項，哪些需要多一層明確 opt-in。
2. 建立 `public readonly struct` 作為對外型別；內部放 `private enum InnerValue : byte` 儲存實際值。
3. 確保 `InnerValue` 的 `0` 是最安全或最保守的預設值，並讓 backing field 使用 `default`。
4. 用 private constructor 接收 `InnerValue`，避免外部自行建構任意值。
5. 一般選項用 top-level static property 暴露；需刻意選擇的選項放進 nested static class。
6. 視需要提供 `IsXxx` 或 `IsInGroup` 之類的意圖檢查 property，讓呼叫端不用碰 raw value。
7. 實作 `Equals`、`GetHashCode`、`==`、`!=`；若目標 codebase 有慣例，可加上 `IEquatable<T>`。
8. 不要加 implicit conversion、public raw enum、public constructor、任意 `FromString`，除非使用者明確要求且有驗證規則。

## 參考素材

- 讀取 `examples/LayeredChoice.cs` 作為泛化 C# 範例。
- 這份範例刻意只使用模糊命名；套用到目標專案時，依目標專案的語意與 coding conventions 改名。

## 檢查清單

- 對外型別是 `readonly struct`，內部 enum 是 private。
- `0` 值與 `default` 狀態是安全預設。
- 一般選項在 top-level static property；需要警覺或明確 opt-in 的選項在 nested static class。
- 呼叫端不需要知道 raw enum、byte、string 或內部儲存值。
- equality/hash 行為只依內部值判斷。
- XML docs、縮排、命名慣例跟目標專案一致。
- skill、prompt、agent、notes 中沒有留下來源專案的真實命名或業務內容。