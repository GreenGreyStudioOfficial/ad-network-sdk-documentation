# Load
Загружает креатив.

Реализация зависит от пользователя.

По завершению загрузки должен вернуть [callback](../listeners/IAdLoadListener/IAdLoadListener.md).

Смотрите также: [ISDKConnector](ISDKConnector.md)

## Объявление:

`void Load(AdType _adType, IAdLoadListener _listener, string _placementId = null)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_adType` | `Тип рекламного объявления`
[IAdLoadListener](../listeners/IAdLoadListener/IAdLoadListener.md) | `_listener` | `Реализация слушателя загрузки`
`string` | `_placementId` | `Плейсмент рекламного креатива`