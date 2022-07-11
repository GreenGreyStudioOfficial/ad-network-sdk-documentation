# LoadErrorType
Перечисления возможных ошибок загрузки.

Смотрите также: [Enums](enums.md)

## Значения

значение | описание
-|-
`UNKNOWN` | Неизвестная ошибка
`CONNECTION_ERROR` | Ошибка соединения
`DATA_PROCESSING_ERROR` | Ошибка обработки данных
`PROTOCOL_ERROR` | Ошибка протокола
`NOT_INITIALIZED_ERROR` | Отсутствует инициализация
`INITIALIZATION_NOT_FINISHED` | Инициализация не завершена
`TO_MANY_VIDEOS_LOADED` | Уже загруженно слишком много видео данного типа
`AVAILABLE_VIDEO_NOT_FOUND` | Сервис предоставления рекламы не нашел соответствующий ролик
`NO_CONTENT` | Нет контента
`NOT_SUPPORTED_AD_TYPE` | Данный тип не поддерживается коннектором
`THIRD_PARTY_CONNECTOR_ERROR` | Ошибка в пользовательском коннекторе
`REQUEST_NOT_CREATED` | Ошибка создания запроса
`NO_CONNECTORS_RECEIVED` | Отсутствуют валидные коннекторы
`WEBVIEW_CONTENT_NOT_LOADED` | Ошибка загрузки WebView

## Namespace:

`GreenGrey.AdNetworkSDK.DataModel.Enums`