# OnShowComplete
Обработчик завершения показа объявления. Вызывается, когда показ успешно завершен.

Смотрите также: [IAdShowListener](IAdShowListener.md)

## Объявление:

`public void OnShowComplete(string _id, string _bidid, ShowCompletionState _showCompletionState);`

тип | имя | описание
-|-|-
`string` | `_id` | Индетификатор показываемого рекламного объявления
`string` | `_bidid` | Индетификатор показываемого рекламного объявления для валидации на сервере
[ShowCompletionState](../../enums/ShowCompletionState.md) | `_showCompletionState` | Статус завершения показа