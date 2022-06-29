# Initialize
Инициализирует работу коннектора.

Реализация зависит от пользователя.

По завершению должен вернуть [callback](../listeners/IAdInitializationListener/IAdInitializationListener.md).

Смотрите также: [ISDKConnector](ISDKConnector.md)

## Объявление:

`void Initialize(IAdInitializationListener _listener)`

тип | имя | описание
-|-|-
[IAdInitializationListener](../listeners/IAdInitializationListener/IAdInitializationListener.md) | `_listener` | Реализация слушателя инициализации