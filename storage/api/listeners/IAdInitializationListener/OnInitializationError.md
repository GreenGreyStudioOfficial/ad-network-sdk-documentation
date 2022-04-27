# OnInitializationError
Обработчик ошибок завершения инициализации. Вызывается, когда инициализация прошла с ошибкой.

Смотрите также: [IAdInitializationListener](IAdInitializationListener.md)

## Объявление:

`public void OnInitializationError(InitializationErrorType _error, string _errorMessage);`

тип | имя | описание
-|-|-
[InitializationErrorType](../../enums/InitializationErrorType.md) | `_error` | Тип ошибки
`string` | `_errorMessage` | Информация об ошибке