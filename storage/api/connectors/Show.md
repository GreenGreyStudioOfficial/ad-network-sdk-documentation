# Show
Отображает креатив.

Реализация зависит от пользователя.

По завершению показа должен вернуть [callback](../listeners/IAdShowListener/IAdShowListener.md).

Смотрите также: [ISDKConnector](ISDKConnector.md)

## Объявление:

`void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_adType` | `Тип рекламного объявления`
[IAdShowListener](../listeners/IAdShowListener/IAdShowListener.md) | `_listener` | `Реализация слушателя показа`
`string` | `_placementId` | `Плейсмент рекламного креатива`