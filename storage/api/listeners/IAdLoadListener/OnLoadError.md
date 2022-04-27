# OnLoadError
Обработчик ошибок загрузки рекламного объявления. Вызывается, когда загрузка прошла c ошибкой.

Смотрите также: [IAdLoadListener](IAdLoadListener.md)

## Объявление:

`public void OnLoadError(LoadErrorType _error, string _id, string _errorMessage);`

тип | имя | описание
-|-|-
[LoadErrorType](../../enums/LoadErrorType.md) | `_error` | Тип ошибки
`string` | `_id` | Идентификатор реквеста, который не удалось загрузить
`string` | `_errorMessage` | Информация об ошибке