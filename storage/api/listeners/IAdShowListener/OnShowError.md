# OnShowError
Обработчик ошибок показа рекламного объявления. Вызывается, когда показ прошел c ошибкой.

Смотрите также: [IAdShowListener](IAdShowListener.md)

## Объявление:

`public void OnShowError(string _id, ShowErrorType _error, string _errorMessage);`

тип | имя | описание
-|-|-
`string` | `_id` | Индетификатор показываемого рекламного объявления
[ShowErrorType](../../enums/ShowErrorType.md) | `_error` | Тип ошибки
`string` | `_errorMessage` | Информация об ошибке