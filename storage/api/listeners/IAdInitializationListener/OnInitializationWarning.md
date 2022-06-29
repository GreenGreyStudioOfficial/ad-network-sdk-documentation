# OnInitializationWarning
Обработчик некритических ошибок завершения инициализации. Вызывается, когда инициализация завершилась успешно, но есть ошибки в пользовательских коннекторах.

Смотрите также: [IAdInitializationListener](IAdInitializationListener.md)

## Объявление:

`void OnInitializationWarning(InitializationWarningType _warningType, string _warningMessage)`

тип | имя | описание
-|-|-
[InitializationWarningType](../../enums/InitializationWarningType.md) | `_warningType` | Тип ошибки
`string` | `_warningMessage` | Информация об ошибке