# AdNetworkInitParams
Параметры инициализации рекламной сети Green Grey

Смотрите также: [models](models.md)

## Свойства:

тип | имя | описание
-|-|-
string | `m_gameId` | Айди приложения
bool | `m_isTestMode` | Флаг тестового режима
bool | `m_autoLoadEnabled` | Разрешена ли автоматическая загрузка креативов после инициализации
List<[AdType](../enums/AdType.md)> | `m_adTypesForAutoLoad` | Список [AdType](../enums/AdType.md) для автоматической загрузки

## Конструктор:

`public AdNetworkInitParams(string _gameId, bool _isTestMode, bool _autoLoadEnabled, List<AdType> _adTypesForAutoLoad)`

## Namespace:

`GreenGrey.AdNetworkSDK.DataModel`