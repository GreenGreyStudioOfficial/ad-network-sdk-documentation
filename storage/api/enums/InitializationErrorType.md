# InitializationErrorType
Перечисления возможных ошибок инициализации.

Смотрите также: [Enums](enums.md)

## Значения:

значение | описание
-|-
`UNKNOWN` | Неизвестная ошибка
`GAME_ID_IS_NULL_OR_EMPTY` | Задан пустой айди игры
`SDK_ALREADY_INITIALIZED` | Инициализация уже была произведена
`INITIALIZE_PROCESS_ALREADY_STARTED` | Процесс инициализации уже запущен
`GGAD_CONNECTOR_INITIALIZE_FAILED` | Инициализация коннектора провалилась
`THIRD_PARTY_CONNECTOR_ERROR` | Ошибка в пользовательском коннекторе
`NO_INTERNET_CONNECTION` | Отсутствует интернет соединение
`INVALID_GAME_ID` | Невалидный айди игры

## Namespace:

`GreenGrey.AdNetworkSDK.DataModel.Enums`