# OnShowError
Обработчик ошибок показа рекламного объявления. Вызывается, когда показ прошел c ошибкой.

Смотрите также: [IAdShowListener](IAdShowListener.md)

## Объявление:

`void OnShowError(AdType _adType, ShowErrorType _error, string _errorMessage)`

тип | имя | описание
-|-|-
[AdType](../../enums/AdType.md) | `_adType` | Тип рекламного креатива
[ShowErrorType](../../enums/ShowErrorType.md) | `_error` | Тип ошибки
`string` | `_errorMessage` | Информация об ошибке