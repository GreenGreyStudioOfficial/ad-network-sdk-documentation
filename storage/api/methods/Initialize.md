# Initialize
Инициализирует работу SDK.

На вход нужно передать ID игрового проекта, зарегистрированного на странице сервиса, адрес хоста (также получается на странице сервиса), флаг режима работы и реализацию слушателя [IAdInitializationListener](../listeners/IAdInitializationListener/IAdInitializationListener.md)

Без инициализации методы [AdNetworkSDK.Load](Load.md), [AdNetworkSDK.LazyLoad](LazyLoad.md) и [AdNetworkSDK.Show](Show.md) не отработают корректно и будут сообщать своим слушателям об ошибках [LoadErrorType.NOT_INITIALIZED_ERROR](../enums/LoadErrorType.md) и [ShowErrorType.NOT_INITIALIZED_ERROR](../enums/ShowErrorType.md) соответственно.

При попытке повторной инициализации, вызовется callback ее слушателя [IAdInitializationListener.OnInitializationError](../listeners/IAdInitializationListener/OnInitializationError.md) с ошибкой [InitializationErrorType.SDK_ALREADY_INITIALIZED](../enums/InitializationErrorType.md).

В случае, если на вход метода пришел пустой ID проекта, вызовется callback ее слушателя [IAdInitializationListener.OnInitializationError](../listeners/IAdInitializationListener/OnInitializationError.md) с ошибкой [InitializationErrorType.GAME_ID_IS_NULL_OR_EMPTY](../enums/InitializationErrorType.md).

В случае, если на вход метода пришел пустой адрес хоста, вызовется callback ее слушателя [IAdInitializationListener.OnInitializationError](../listeners/IAdInitializationListener/OnInitializationError.md) с ошибкой [InitializationErrorType.AD_SERVER_HOST_IS_NULL_OR_EMPTY](../enums/InitializationErrorType.md).

Смотрите также: [AdNetworkSDK](AdNetworkSDK.md)

## Объявление:

`public static void Initialize(string _gameId, string _adServerHost, bool _isTestMode, IAdInitializationListener _listener)`

тип | имя | описание
-|-|-
`string` | `_gameId` | ID игрового проекта
`string` | `_adServerHost` | Адрес хоста сервиса
`bool` | `_isTestMode` | Флаг режима работы
[IAdInitializationListener](../listeners/IAdInitializationListener/IAdInitializationListener.md) | `_listener` | Реализация слушателя инициализации