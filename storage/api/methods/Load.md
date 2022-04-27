# Load
Загружает доступное рекламное объявление из сети.

На вход нужно передать [тип рекламного объявления](../enums/AdType.md) и реализацию слушателя [IAdLoadListener](../listeners/IAdLoadListener/IAdLoadListener.md)

Метод запускает процесс загрузки и возвращает ID запроса, который в дальнейшем будет использоваться для показа ролика.

По завершению процесса загрузки вызовется [callback слушателя](../listeners/IAdLoadListener/IAdLoadListener.md) с тем же ID, который вернул метод.

Метод сохраняет данные рекламного объявления в кэш, который самостоятельно очищается после показа рекламы, либо по истечению срока годности ролика.  
Существуют ограничения по кол-ву загрузок, которые регулируются сервером и обновляются при каждой загрузке. Если лимит достигнут, [слушатель](../listeners/IAdLoadListener/IAdLoadListener.md) будет оповещен с помощью ошибки [LoadErrorType.TO_MANY_VIDEOS_LOADED](../enums/LoadErrorType.md). Лимит для каждого типа рекламы считается отдельно.

Смотрите также: [AdNetworkSDK](AdNetworkSDK.md)

## Объявление:

`public static string Load(AdType _advertiseType, IAdLoadListener _listener)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_advertiseType` | `Тип рекламного объявления`
[IAdLoadListener](../listeners/IAdLoadListener/IAdLoadListener.md) | `_listener` | `Реализация слушателя загрузки`