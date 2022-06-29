# Load
Загружает доступное рекламное объявление из сети.

На вход нужно передать [тип рекламного объявления](../enums/AdType.md), реализацию слушателя [IAdLoadListener](../listeners/IAdLoadListener/IAdLoadListener.md) и placementId рекламного креатива. В текущей версии placementId используется только для работы с [коннекторами](../connectors/connectors.md).

Метод запускает процесс загрузки рекламного креатива. По завершению процесса загрузки вызовется [callback слушателя](../listeners/IAdLoadListener/IAdLoadListener.md) с тем же [типом рекламного объявления](../enums/AdType.md), с которым был вызван метод Load.

Метод сохраняет данные рекламного объявления в кэш, который самостоятельно очищается после показа рекламы, либо по истечению срока годности ролика.  
Существуют ограничения по кол-ву загрузок, которые регулируются сервером и обновляются при каждой загрузке. Лимит для каждого типа рекламы считается отдельно. Если в [AdNetworkInitParams](../models/AdNetworkInitParams.md) был передан список [типов рекламного объявления](../enums/AdType.md) для автозагрузки, то [AdNetworkSDK](AdNetworkSDK.md) будет поддерживать максимально возможное количество креативов в кеше приложения ([AdNetworkSDK](AdNetworkSDK.md) не реализует данный механизм в [коннекторах](../connectors/connectors.md)).

Обратите внимание, что при повторном вызове с одим и тем же [типом рекламного объявления](../enums/AdType.md) не происходит загрузка нового креатива. Если данный тип уже закеширован, то вызовется [callback слушателя](../listeners/IAdLoadListener/IAdLoadListener.md).

В случае наличия [коннекторов](../connectors/connectors.md) алгоритм работы выглядит следующим образом: Если [тип рекламного объявления](../enums/AdType.md) поддерживается рекламным сервисом, то он пытается его загрузить. В случае ошибки [AdNetworkSDK](AdNetworkSDK.md) обращается к следующему [коннектору](../connectors/connectors.md)  в списке. После завершения работы [коннекторов](../connectors/connectors.md) вызовется [callback слушателя](../listeners/IAdLoadListener/IAdLoadListener.md) с тем же [типом рекламного объявления](../enums/AdType.md), с которым был вызван метод Load.

Смотрите также: [AdNetworkSDK](AdNetworkSDK.md)

## Объявление:

`public static void Load(AdType _adType, IAdLoadListener _listener, string _placementId)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_adType` | `Тип рекламного объявления`
[IAdLoadListener](../listeners/IAdLoadListener/IAdLoadListener.md) | `_listener` | `Реализация слушателя загрузки`
`string` | `_placementId` | `Плейсмент рекламного креатива`