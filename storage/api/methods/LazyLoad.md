# LazyLoad
Загружает доступное рекламное объявление из сети или кэша.

Метод по своей работе аналогичен методу [AdNetworkSDK.Load](Load.md), однако, в случае, если в кэше уже загружено рекламное объявление заданного типа, то возвращается оно.

Это позволяет избежать обработку ошибки [LoadErrorType.TO_MANY_VIDEOS_LOADED](../enums/LoadErrorType.md), однако сохранить несколько роликов в кэш с помощью этого метода невозможно.

Смотрите также: [AdNetworkSDK](AdNetworkSDK.md)

## Объявление:

`public static string LazyLoad(AdType _advertiseType, IAdLoadListener _listener)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_advertiseType` | `Тип рекламного объявления`
[IAdLoadListener](../listeners/IAdLoadListener/IAdLoadListener.md) | `_listener` | `Реализация слушателя загрузки`