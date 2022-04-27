# OnShowComplete
Обработчик завершения показа объявления. Вызывается, когда показ успешно завершен.

Смотрите также: [IAdShowListener](IAdShowListener.md)

## Объявление:

`public void OnShowComplete(string _id, ShowCompletionState _showCompletionState);`

тип | имя | описание
-|-|-
`string` | `_id` | Индетификатор показываемого рекламного объявления
[ShowCompletionState](../../enums/ShowCompletionState.md) | `_showCompletionState` | Статус завершения показа