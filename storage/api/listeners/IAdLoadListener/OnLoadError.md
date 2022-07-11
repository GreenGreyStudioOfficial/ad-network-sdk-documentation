# OnLoadError
Обработчик ошибок загрузки рекламного объявления. Вызывается, когда загрузка прошла c ошибкой.

Смотрите также: [IAdLoadListener](IAdLoadListener.md)

## Объявление:

`void OnLoadError(AdType _adType, LoadErrorType _error, string _errorMessage)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_adType` | Тип рекламного креатива
[LoadErrorType](../../enums/LoadErrorType.md) | `_error` | Тип ошибки
`string` | `_errorMessage` | Информация об ошибке