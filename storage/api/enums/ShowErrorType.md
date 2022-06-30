# ShowErrorType
Перечисления возможных ошибок показа роликов.

Смотрите также: [Enums](enums.md)

## Значения:

значение | описание
-|-
`UNKNOWN` | Неизвестная ошибка
`ID_NOT_FOUND` | Видео с таким id не найдено
`VIDEO_CACHE_NOT_FOUND` | Файл видео не найден
`VIDEO_DATA_NOT_FOUND` | Метаданные видео не найдены
`NOT_INITIALIZED_ERROR` | Отсутствует инициализация
`INITIALIZATION_NOT_FINISHED` | Процесс инициализации не завершен
`VIDEO_WAS_DELETED` | Видео было удалено по причине просроченного токена
`VIDEO_PLAYER_ERROR` | Ошибка видео плеера
`NO_LOADED_CONTENT` | Нет загруженного контента
`THIRD_PARTY_CONNECTOR_ERROR` | Ошибка в пользовательском коннекторе
`CONNECTORS_NOT_RECEIVED` | Нет валидных коннекторов
`NOT_SUPPORTED_AD_TYPE` | Не поддерживаемый тип креатива

## Namespace:

`GreenGrey.AdNetworkSDK.DataModel.Enums`