# ISDKConnector
Интерфейс коннектора сторонних рекламных sdk. Позволяет управлять всеми рекламными sdk интегрированными в приложение через [AdNetworkSDK](../methods/AdNetworkSDK.md)

## Публичные методы:
метод | описание
-|-
[Initialize](Initialize.md) | Инициализация коннектора
[Load](Load.md) | Загрузка креатива
[Show](Show.md) | Показ креатива
[GetSupportedAdTypes](GetSupportedAdTypes.md) | Получить список поддерживаемых [типов](../enums/AdType.md)
[GetSdkId](GetSdkId.md) | Получить айди

## Публичные свойства:
тип | имя | описание
-|-|-
`bool` | `isInitialized` | Инициализирован ли коннектор
`bool` | `isInitializeStarted` | Начат ли процесс инициализации

## Namespace:

`GreenGrey.AdNetworkSDK.Interfaces.Connector`