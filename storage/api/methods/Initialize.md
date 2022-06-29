# Initialize
Инициализирует работу SDK.

На вход нужно передать параметры инициализации сдк [AdNetworkInitParams](../models/AdNetworkInitParams.md), реализацию слушателя [IAdInitializationListener](../listeners/IAdInitializationListener/IAdInitializationListener.md) и массив коннекторов, реализующих интерфейс [ISDKConnector](../connectors/ISDKConnector.md) для взаимодействия со сторонними рекламными sdk.

Без инициализации методы [AdNetworkSDK.Load](Load.md) и [AdNetworkSDK.Show](Show.md) не отработают корректно и будут сообщать своим слушателям об ошибках [LoadErrorType.NOT_INITIALIZED_ERROR](../enums/LoadErrorType.md) и [ShowErrorType.NOT_INITIALIZED_ERROR](../enums/ShowErrorType.md) соответственно.

Если инициализация запущена, но не завершена методы [AdNetworkSDK.Load](Load.md) и [AdNetworkSDK.Show](Show.md) не отработают корректно и будут сообщать своим слушателям об ошибках [LoadErrorType.INITIALIZATION_NOT_FINISHED](../enums/LoadErrorType.md) и [ShowErrorType.INITIALIZATION_NOT_FINISHED](../enums/ShowErrorType.md) соответственно.

Если sdk успешно инициализирован, то при попытке повторной инициализации, вызовется callback ее слушателя [IAdInitializationListener.OnInitializationError](../listeners/IAdInitializationListener/OnInitializationError.md) с ошибкой [InitializationErrorType.SDK_ALREADY_INITIALIZED](../enums/InitializationErrorType.md).

Если sdk в процессе инициализации, то при попытке повторной инициализации, вызовется callback ее слушателя [IAdInitializationListener.OnInitializationError](../listeners/IAdInitializationListener/OnInitializationError.md) с ошибкой [InitializationErrorType.INITIALIZE_PROCESS_ALREADY_STARTED](../enums/InitializationErrorType.md).

 Если при инициализации в [AdNetworkInitParams](../models/AdNetworkInitParams.md) был передан некорректный GAME_ID(null, "", invalid), то будет вызван callback слушателя [IAdInitializationListener.OnInitializationError](../listeners/IAdInitializationListener/OnInitializationError.md) с ошибкой [InitializationErrorType.GGAD_CONNECTOR_INITIALIZE_FAILED](../enums/InitializationErrorType.md).

В случае наличия аргументов [ISDKConnector[]](../connectors/ISDKConnector.md) процесс инициализации выстраивается следующим образом: сначала вызывается инициализация [AdNetworkSDK](AdNetworkSDK.md) и только в случае успешной инициализации вызывается инициализация коннекторов переданных в аргументах.

Если инициализация всех коннекторов прошла успешно, то вызывается callback [IAdInitializationListener.OnInitializationComplete](../listeners/IAdInitializationListener/OnInitializationComplete.md).

Если инициализация хотя бы одного коннектора провалилась, то вызывается callback [IAdInitializationListener.OnInitializationComplete](../listeners/IAdInitializationListener/OnInitializationComplete.md) и  [IAdInitializationListener.OnInitializationWarning](../listeners/IAdInitializationListener/OnInitializationWarning.md).

Если инициализация всех коннекторов провалилась, но [AdNetworkSDK](AdNetworkSDK.md) был успешно проинициализирован, то также будет вызван callback [IAdInitializationListener.OnInitializationComplete](../listeners/IAdInitializationListener/OnInitializationComplete.md) и  [IAdInitializationListener.OnInitializationWarning](../listeners/IAdInitializationListener/OnInitializationWarning.md).

Смотрите также: [AdNetworkSDK](AdNetworkSDK.md)

## Объявление:

`public static void Initialize(AdNetworkInitParams _adNetworkInitParams, IAdInitializationListener _listener, ISdkConnector[] _otherConnectors = null)`

тип | имя | описание
-|-|-
[AdNetworkInitParams](../models/AdNetworkInitParams.md) | `_adNetworkInitParams` | Параметры инициализации рекламной сетки Green Grey
[IAdInitializationListener](../listeners/IAdInitializationListener/IAdInitializationListener.md) | `_listener` | Реализация слушателя инициализации
[ISDKConnector[]](../connectors/ISDKConnector.md) | `_otherConnectors` | Массив реализаций коннекторов со сторонними sdk