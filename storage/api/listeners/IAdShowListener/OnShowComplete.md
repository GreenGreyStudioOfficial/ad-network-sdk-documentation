# OnShowComplete
Обработчик завершения показа объявления. Вызывается, когда показ успешно завершен.

Смотрите также: [IAdShowListener](IAdShowListener.md)

## Объявление:

`void OnShowComplete(AdType _adType, ShowCompletionState _showCompletionState, string _validationId)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_adType` | Тип рекламного креатива
[ShowCompletionState](../../enums/ShowCompletionState.md) | `_showCompletionState` | Статус завершения показа
`string` | `_validationId` | Идентификатор показываемого рекламного объявления для валидации на сервере.