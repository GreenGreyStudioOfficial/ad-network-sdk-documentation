# Show
Показывает загруженное рекламное объявление.

На вход нужно передать [тип рекламного объявления](../enums/AdType.md), реализацию слушателя [IAdShowListener](../listeners/IAdShowListener/IAdShowListener.md) и placementId рекламного креатива. В текущей версии placementId используется только для работы с [коннекторами](../connectors/connectors.md).

Метод запускает процесс показа ролика.

Перед показом вызывается callback [IAdShowListener.OnShowStart](../listeners/IAdShowListener/OnShowStart.md)

По окончанию успешного показа у слушателя вызовется callback [IAdShowListener.OnShowComplete](../listeners/IAdShowListener/OnShowComplete.md), сообщив о статусе завершения.  
Это важно, если, например, объявление имело тип [AdType.REWARDED](../enums/AdType.md), так как в этом случае для присуждения награды нужно знать был ли ролик досмотрен до конца ([ShowCompletionState.SHOW_COMPLETE_BY_CLOSE_BUTTON](../enums/ShowCompletionState.md)) или пропущен ([ShowCompletionState.SHOW_COMPLETE_BY_SKIP_BUTTON](../enums/ShowCompletionState.md)).

В случае, если во время показа произошла ошибка, будет вызван callback [IAdShowListener.OnShowError](../listeners/IAdShowListener/OnShowError.md).  
Так, например, если кэш ролика просрочился перед вызовом, то на вход методу передастся ошибка [ShowErrorType.VIDEO_WAS_DELETED](../enums/ShowErrorType.md)

После завершения показа, вне зависимости от его успешности, кэш объявления очищается.

Смотрите также: [AdNetworkSDK](AdNetworkSDK.md)

## Объявление:

`public static void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)`

тип | имя | описание
-|-|-
[AdType](../enums/AdType.md) | `_adType` | `Тип рекламного объявления`
[IAdShowListener](../listeners/IAdShowListener/IAdShowListener.md) | `_listener` | `Реализация слушателя показа`
string | `_placementId` | `Плейсмент рекламного креатива`