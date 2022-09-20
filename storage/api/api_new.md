# Описание API
В данном разделе собрана техническая документация классов, интерфейсов, перечислений и тд, доступных пользователям библиотеки.

### Languages
1. [Russian](#russian)
2. [English](#english)

_____
### Russian
_____
<a name="russian"></a>
# Статичный класс AdNetworkSDK
Статичный класс **AdNetworkSDK** - публичный интерфейс для взаимодействия с **SDK**.

Включает в себя следующие публичные методы:

• [Initialize](#initialize): инициализации работы **SDK**;
• [Load](#load): загрузка доступного рекламного объявления из сети или из кэша;
• [Show](#show): показ загруженного рекламного объявления.

## Содержание

- [Метод Initialize](#ru_initialize)
- [Метод Load](#ru_load)
- [Метод Show](#ru_show)
- [Слушатели](#ru_listeners)
- [Слушатель инициализации](#ru_l_initialization)
- [Слушатель загрузки](#ru_l_load)
- [Слушатель показа](#ru_l_show)
- [Модель объекта AdNetworkInitParams](#ru__AdNetworkInitParams)
- [Коннекторы ISdkConnector](#ru_ISdkConnector)

## Метод Initialize <a name = "ru_initialize"></a>

Метод **Initialize** инициализирует работу **SDK**.

На вход передаются параметры инициализации **SDK** [AdNetworkInitParams](#AdNetworkInitParams), реализация слушателя [IAdInitializationListener](#IAdInitializationListener) и массив коннекторов, реализующих интерфейс [ISDKConnector](#ISDKConnector) для взаимодействия со сторонними рекламными **SDK**.

Без инициализации методы [AdNetworkSDK.Load](#load) и [AdNetworkSDK.Show](#show) не отработают корректно и будут сообщать своим слушателям об ошибках
**LoadErrorType.NOT_INITIALIZED_ERROR** и **ShowErrorType.NOT_INITIALIZED_ERROR** соответственно.

Если инициализация запущена, но не завершена, методы [AdNetworkSDK.Load](#load) и [AdNetworkSDK.Show](#show) не отработают корректно и будут сообщать своим слушателям об ошибках
**LoadErrorType.INITIALIZATION_NOT_FINISHED** и **ShowErrorType.INITIALIZATION_NOT_FINISHED**
соответственно.

Если **SDK** успешно инициализирован, то при попытке повторной инициализации, вызовется **callback** ее слушателя [IAdInitializationListener.OnInitializationError](#OnInitializationError) с ошибкой **InitializationErrorType.SDK_ALREADY_INITIALIZED**.

Если **SDK** в процессе инициализации, то при попытке повторной инициализации, вызовется **callback** ее слушателя [IAdInitializationListener.OnInitializationError](#OnInitializationError) с ошибкой **InitializationErrorType.INITIALIZE_PROCESS_ALREADY_STARTED**.

Если при инициализации в [AdNetworkInitParams](#AdNetworkInitParams) был передан некорректный **GAME_ID** (null, "", invalid), то будет вызван **callback** слушателя [IAdInitializationListener.OnInitializationError](#OnInitializationError) с ошибкой **InitializationErrorType.GGAD_CONNECTOR_INITIALIZE_FAILED**.

В случае наличия аргументов [ISDKConnector[]](#ISDKConnector) процесс инициализации выстраивается следующим образом: сначала вызывается инициализация **AdNetworkSDK** и только в случае успешной инициализации вызывается инициализация коннекторов переданных в аргументах.

Если инициализация всех коннекторов прошла успешно, то вызывается **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete).

Если инициализация хотя бы одного коннектора провалилась, то вызывается **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete) и
[IAdInitializationListener.OnInitializationWarning](#OnInitializationWarning).

Если инициализация всех коннекторов провалилась, но **AdNetworkSDK** был успешно проинициализирован, то также будет вызван **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete) и
[IAdInitializationListener.OnInitializationWarning](#OnInitializationWarning).

**Объявление**:

```
public static void Initialize(AdNetworkInitParams _adNetworkInitParams, IAdInitializationListener _listener, ISdkConnector[] _otherConnectors = null)
```

где:

| Тип| Имя| Описание|
|---|---|---|
|[AdNetworkInitParams](#AdNetworkInitParams)| adNetworkInitParams| Параметры инициализации рекламной сети **Green Grey**|
|[IAdInitializationListener](#IAdInitializationListener)| listener| Реализация слушателя инициализации|
| [ISdkConnector[]](#ISdkConnector) | otherConnectors | Массив реализаций коннекторов со сторонними **SDK**|

## Метод Load <a name = "ru_load"></a>

Метод **Load** загружает доступное рекламное объявление из сети или из кеша.

На вход нужно передать [тип рекламного объявления](#adtype), реализацию слушателя [IAdLoadListener](#IAdLoadListener) и **placementId** рекламного креатива. В текущей версии IAdLoadListener используется только для работы с коннекторами.

Метод запускает процесс загрузки рекламных объявлений. По завершению процесса загрузки вызовется **callback** слушателя с тем же типом рекламного объявления, с которым был вызван метод **Load**.

Метод сохраняет данные рекламного объявления в кэш, который самостоятельно очищается после показа рекламы, либо по истечению срока годности ролика.

Существуют ограничения по количеству загрузок, которые регулируются сервером и обновляются при каждой загрузке. Лимит для каждого типа рекламы считается отдельно. Если в [AdNetworkInitParams](#AdNetworkInitParams) был передан список типов рекламного объявления для автозагрузки, то AdNetworkSDK будет поддерживать максимально возможное количество креативов в кэше приложения (**AdNetworkSDK** не реализует данный механизм в коннекторах).

В случае наличия коннекторов алгоритм работы выглядит следующим образом:

1. Если тип рекламного объявления поддерживается рекламным сервисом, то он пытается его загрузить.
2. В случае ошибки **AdNetworkSDK** обращается к следующему коннектору в списке.
3. После завершения работы коннекторов вызовется **callback** слушателя с тем же типом рекламного объявления, с которым был вызван метод **Load**.

**Объявление**:

```
public static void Load(AdType _adType, IAdLoadListener _listener, string _placementId)
```

где:

`AdType` - тип рекламного объявления (см. [AdType](#adtype));

`IAdLoadListener` - реализация слушателя загрузки (см. [IAdLoadListener](#IAdLoadListener));

`string _placementId` - плейсмент рекламного объявления.

## Метод Show <a name = "ru_show"></a>

Показывает загруженное рекламное объявление.

На вход нужно передать [тип рекламного объявления](#adtype), реализацию слушателя [IAdShowListener](#IAdShowListener) и **placementId** рекламного креатива. В текущей версии **placementId** используется только для работы с коннекторами.

Метод запускает процесс показа ролика.

Перед показом вызывается **callback** [IAdShowListener.OnShowStart](#OnShowStart)

По окончанию успешного показа у слушателя вызовется **callback** [IAdShowListener.OnShowComplete](#OnShowComplete), сообщив о статусе завершения.

Это важно, если, например, объявление имело тип `AdType.REWARDED`. Так как в этом случае для присуждения награды нужно знать был ли ролик досмотрен до конца (`ShowCompletionState.SHOW_COMPLETE_BY_CLOSE_BUTTON`) или пропущен (`ShowCompletionState.SHOW_COMPLETE_BY_SKIP_BUTTON`).

В случае, если во время показа произошла ошибка, будет вызван **callback** [IAdShowListener.OnShowError](#OnShowError).
Так, например, если срок кэша ролика истек до вызова, то на вход методу передастся ошибка `ShowErrorType.VIDEO_WAS_DELETED`.

После завершения показа, вне зависимости от его успешности, кэш объявления очищается.

**Объявление**:

```
public static void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)
```

где:

`AdType` - тип рекламного объявления (см. [AdType](#adtype));

`IAdShowListener` - реализация слушателя показа (см. [IAdShowListener](#IAdShowListener));

`string _placementId` - плейсмент рекламного объявления.

# Слушатели <a name = "ru_listener"></a>

Слушатели (или **listeners**) - это интерфейсы, которые дают возможность контролировать процессы инициализации, загрузки и показа рекламных объявлений.

В системе используется три вида слушателей:

• [Слушатель инициализации IAdInitializationListener](#l_initialization);
• [Слушатель загрузки IAdLoadListener](#l_load);
• [Слушатель показа IAdShowListener](#l_show).

## Слушатель инициализации <a name = "ru_l_initialization"></a>

Интерфейс слушателя инициализации **SDK** **IAdInitializationListener** используется для контроля выполнения процесса инициализации.

Использует следующие публичные методы:

• [OnInitializationComplete](#OnInitializationComplete): обработчик завершения инициализации;
• [OnInitializationError](#OnInitializationError): обработчик ошибок инициализации;
• [OnInitializationWarning](#OnInitializationWarning): обработчик некритических ошибок инициализации.

### OnInitializationComplete <a name = "ru_OnInitializationComplete"></a>

Обработчик завершения инициализации вызывается, когда инициализация прошла успешно.

**Объявление**:

```
public void OnInitializationComplete();
```

### OnInitializationWarning <a name = "ru_OnInitializationWarning"></a>

Обработчик некритических ошибок завершения инициализации вызывается, когда инициализация прошла успешно, но есть ошибки в пользовательских коннекторах.

**Объявление**:

```
void OnInitializationWarning(InitializationWarningType _warningType, string _warningMessage)
```

где:

| Тип| Имя| Описание|
|---|---|---|
| InitializationWarningType | warningType| Тип ошибки| 
| string| warningMessage| Информация об ошибке|


**Варианты предупреждений**:

| начение| Описание|
|---|---|
|UNKNOWN| Неизвестное предупреждение|
|NOT_ALL_CONNECTORS_WAS_INITIALIZED| Ошибка инициализации одного или нескольких коннекторов| 
|THIRD_PARTY_CONNECTOR_WARNING| Предупреждение со стороны стороннего коннектора|

### OnInitializationError <a name = "ru_OnInitializationError"></a> 

Обработчик ошибок завершения инициализации вызывается, когда инициализация прошла с ошибкой.

**Объявление**:

```
public void OnInitializationError(InitializationErrorType _error, string _errorMessage);
```

| Тип| Имя| Описание|
|---| --- |---|
|InitializationErrorType| error| Тип ошибки|
|string| errorMessage| Информация об ошибке|

**Варианты ошибок**:

| Значение| Описание| 
|---|---|
| UNKNOWN| Неизвестная ошибка|
|GAME_ID_IS_NULL_OR_EMPTY | Задан пустой идентификатор игры| 
| SDK_ALREADY_INITIALIZED| Инициализация уже была проведена|
| INITIALIZE_PROCESS_ALREADY_STARTED| Процесс инициализации уже запущен| 
| GGAD_CONNECTOR_INITIALIZE_FAILED| Инициализация коннекторов провалилась|
| THIRD_PARTY_CONNECTOR_ERROR| Ошибка в пользовательском коннекторе| 
| INVALID_GAME_ID| Недействительный идентификатор игры|

## Слушатель загрузки <a name = "ru_l_load"></a>

Интерфейс слушателя загрузки **IAdLoadListener** используется для контроля выполнения процесса загрузки.

Использует следующие публичные методы:

• [OnLoadComplete](#OnLoadComplete) - обработчик завершения загрузки;
• [OnLoadError](#OnLoadError) - обработчик ошибок загрузки рекламных объявлений.

### OnLoadComplete <a name = "ru_OnLoadComplete"></a> 

Обработчик завершения загрузки вызывается, когда загрузка прошла успешно.

**Объявление**:

```
void OnLoadComplete(AdType _adType) 
```

где: 

| Тип| Имя| Описание|
|---|---|---|
| AdType| AdType| Тип рекламного объявления (см. [AdType](#adtype))|

### OnLoadError <a name = "ru_OnLoadError"></a> 

Обработчик ошибок загрузки вызывается, когда загрузка прошла с ошибкой.

**Объявление**:

```
void OnLoadError(AdType _adType, LoadErrorType _error, string _errorMessage)
```

где: 

| Тип| Имя| Описание|
|---|---|---|
| AdType| AdType| Тип рекламного объявления (см. [AdType](#adtype))|
|LoadErrorType | error| Тип ошибки|
|string | errorMessage| Сообщение об ошибке|

**Варианты типа ошибки**:

| Значение| Описание|
|---|---|
|UNKNOWN| Неизвестная ошибка|
| CONNECTION_ERROR| Ошибка соединения| 
| DATA_PROCESSING_ERROR| Ошибка обработки данных|
| PROTOCOL_ERROR| Ошибка протокола|
| NOT_INITIALIZED_ERROR| Отсутствие инициализации|
| INITIALIZATION_NOT_FINISHED| Инициализация не завершена|
| TO_MANY_VIDEOS_LOADED|  Превышен лимит загрузки видео данного типа|
| AVAILABLE_VIDEO_NOT_FOUND| Сервис предоставления рекламы не нашел соответствующий ролик|
| NO_CONTENT| Нет контента|
| NOT_SUPPORTED_AD_TYPE| Данный тип не поддерживается коннектором|
| THIRD_PARTY_CONNECTOR_ERROR| Ошибка в пользовательском коннекторе|
|REQUEST_NOT_CREATED| Ошибка создания запроса|
|NO_CONNECTORS_RECEIVED| Отсутствие валидных коннекторов| 
| WEBVIEW_CONTENT_NOT_LOADED| Ошибка загрузки WebView|


## Слушатель показа <a name = "ru_l_show"></a>

Интерфейс слушателя показа рекламного объявления **IAdShowListener** используется для контроля выполнения процесса показа.

Поддерживает следующие публичные методы:

• [OnShowStart](#OnShowStart) - обработчик начала показа рекламного объявления;
• [OnShowComplete](#OnShowComplete) - обработчик завершения показа объявления;
• [OnShowError](#OnShowError) - обработчик ошибок показа рекламного объявления.

### OnShowStart <a name = "ru_OnShowStart"></a> 

Обработчик начала показа рекламного объявления вызывается перед началом показа.

**Объявление**:

```
void OnShowStart(AdType _adType)
```

где:

|Тип| Имя| Описание|
|---|---|---|
|AdType | AdType | Тип рекламного объявления(см. [AdType)](#adtype)|

### OnShowComplete <a name = "ru_OnShowComplete"></a> 

Обработчик завершения пока рекламного объявления вызывается, когда показ прошел успешно.

**Объявление**:

```
void OnShowComplete(AdType _adType, ShowCompletionState _showCompletionState, string _validationId)
```

где:

| Тип| Имя| Описание|
|---|---|---|
|AdType | AdType | Тип рекламного объявления(см. [AdType](#adtype))|
|ShowCompletionState | ShowCompletionState | Статус завершения показа рекламы|
|string | validationId| Идентификатор показываемого рекламного объявления для валидации на сервере|

**Варианты статуса завершения показа**:

| Значение| Описание | 
|---|---|
|SHOW_COMPLETE_BY_CLOSE_BUTTON| Завершение показа по кнопке **Закрыть**|
| SHOW_COMPLETE_BY_SKIP_BUTTON| Завершение показа по кнопке **Пропустить**|

### OnShowError <a name = "ru_OnShowError"></a>  

Обработчик ошибок показа рекламного объявления вызывается, когда показ прошел с ошибкой.

**Объявление**:

```
void OnShowError(AdType _adType, ShowErrorType _error, string _errorMessage)
```

где:

|Тип| Имя| Описание| 
|---|---|---|
| AdType | AdType | Тип рекламного объявления (см. [AdType](#adtype))|
| ShowErrorType | error| Тип ошибки|
| string | errorMessage| Идентификатор показываемого рекламного объявления для валидации на сервере|

**Варианты ошибки показа**:

| Значение| Описание| 
|---|---|
| UNKNOWN| Неизвестная ошибка|
| NOT_INITIALIZED_ERROR| Отсутствует инициализация| 
| INITIALIZATION_NOT_FINISHED| Процесс инициализации не завершен| 
|VIDEO_PLAYER_ERROR| Ошибка видеоплеера|
|NO_LOADED_CONTENT| Нет загруженного контента|
| THIRD_PARTY_CONNECTOR_ERROR| Ошибка в пользовательском коннекторе|
| CONNECTORS_NOT_RECEIVED| Нет валидных коннекторов|
| NOT_SUPPORTED_AD_TYPE| Неподдерживаемый тип рекламного объявления|


# Модели объектов <a name = "ru__AdNetworkInitParams"></a>

В рекламной **SDK** представлена одна модель объектов **AdNetworkInitParams**, в которой собраны параметры инициализации рекламной сети **Green Grey**. 

**Конструктор**:

```
public AdNetworkInitParams(string _gameId, bool _isTestMode, bool _autoLoadEnabled, List<AdType> _adTypesForAutoLoad)
```

**Свойства**:

| Тип| Имя| Описание|
|---|---|---|
| string | gameId| Идентификатор приложения| 
| bool | isTestMode| Флаг тестового режима|
| bool | autoLoadEnabled | Разрешена ли автоматическая загрузка рекламных объявлений после инициализации|
| List AdType | adTypesForAutoLoad| Список AdType для автоматической загрузки|

# Коннекторы <a name = "ru_ISdkConnector"></a>

Интерфейс коннектора сторонних рекламных **SDK** **ISdkConnector** позволяет управлять всеми рекламными **SDK**, интегрированными в приложение через **AdNetworkSDK**.

Использует следующие публичные методы:

| Метод| Описание|
|---|---|
| Initialize| Инициализация коннектора|
| Load| Загрузка рекламного объявления| 
| Show| Показ рекламного объявления| 
| GetSupportedAdTypes| Получение списка поддерживаемых типов рекламных объявлений| 
|GetSdkId| Получение идентификатора|

**Initialize** - метод, который инициализирует работу коннектора. Реализация зависит от пользователя. По завершении должен вернуться **callback**.

**Объявление**:

```
void Initialize(IAdInitializationListener _listener)
```

где:

`IAdInitializationListener _listener` - слушатель инициализации (см. [IAdInitializationListener](#IAdInitializationListener))

**Load** - метод, который загружает рекламные объявления. Реализация зависит от пользователя. По завершении должен вернуться **callback**.

**Объявление**:

```
void Load(AdType _adType, IAdLoadListener _listener, string _placementId = null)
```

где:

`AdType _adType` - тип рекламного объявления:  <a name = "ru_adtype"></a>

| Значение| Описание| 
|---|---|
| INTERSTITIAL| Видео без вознаграждения| 
|REWARDED| Видео с вознаграждением|
| BANNER| Баннер (не реализовано в сервисе, добавлен для работы с коннекторами)| 
|MREC| MREC (не реализовано в сервисе, добавлен для работы с коннекторами)|

`IAdLoadListener _listener` - слушатель загрузки ( см. [IAdLoadListener](#IAdLoadListener))

**Show** - метод, который показывает рекламное объявление. Реализация зависит от пользователя. По завершении должен вернуться **callback**.

**Объявление**:

```
void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)
```

где:

`AdType _adType` - тип рекламного объявления (см. [AdType](#adtype))
`IAdShowListener _listener` - интерфейс слушателя показа рекламного объявления (см. [IAdShowListener](#IAdShowListener)). 

**GetSupportedAdTypes** - метод, который возвращает список поддерживаемых типов. Реализация зависит от пользователя.

**Объявление**:

```
List<AdType> GetSupportedAdTypes()
```

**GetSdkID** - метод, который возвращает идентификатор коннектора. Реализация зависит от пользователя.

**Объявление**: 

```
string GetSdkId()
```

Интерфейс коннектора сторонних рекламных **SDK** имеет следующие публичные свойства:

| Тип| Имя| Описание|
|---|---|---|
| bool| isInitialized| Инициализирован ли коннектор|
| bool| isInitializedStarted| Начат ли процесс инициализации|















 
 
 
<br/><br/>
<br/><br/>
<br/><br/>
<br/><br/>
_____
<a name="english"></a>
### English
_____

# Static class AdNetworkSDK

Static class **AdNetworkSDK**  is a public interface for cooperation with **SDK**.

It contains the following public methods:

• [Initialize](): initialization of **SDK** work;
• [Load](): loading of available advertisements from network and chache;
• [Show](): show of loaded advertisement.

## Contents
- [The method Initialize](#en_initialize)
- [The method Load](#en_load)
- [The method Show](#en_show)
- [Listeners](#en_listeners)
- [Listener of initialization](#en_l_initialization)
- [Listener of loading](#en_l_load)
- [Listener of show](#en_l_show)
- [Object model AdNetworkInitParams](#en_AdNetworkInitParams)
- [Connectors ISdkConnector](#en_ISdkConnector)

## The method Initialize <a name ="en_initialize"></a>

The method Initialize initializes **SDK** work.

Parameters of initialization **SDK** [AdNetworkInitParams](#AdNetworkInitParams), implementation of the listener [IAdInitializationListener]() and an array of connectors implemented the interface [ISDKConnector](#SdkConnector) for cooperation with third-party **SDK** are sent to the method.

The methods [AdNetworkSDK.Load]() and [AdNetworkSDK.Show]() do not work correctly without initialization. So the errors [LoadErrorType.NOT_INITIALIZED_ERROR]() and [ShowErrorType.NOT_INITIALIZED_ERROR]() will be sent to the listeners.

If the initialization is run but not completed yet, the methods [AdNetworkSDK.Load]() and [AdNetworkSDK.Show]() do not work correctly. So the errors [LoadErrorType.INITIALIZATION_NOT_FINISHED]() and [ShowErrorType.INITIALIZATION_NOT_FINISHED]() will be sent to the listeners.

If **SDK** is initialized successfully, the repeated initialization calls callback of its listener [IAdInitializationListener.OnInitializationError](#OnInitializationError) with the error **InitializationErrorType.SDK_ALREADY_INITIALIZED**.

If **SDK** is initializing, the repeated initialization calls callback of its listener [AdInitializationListener.OnInitializationError](#OnInitializationError) with the error **InitializationErrorType.INITIALIZE_PROCESS_ALREADY_STARTED**.

If incorrect **GAME_ID** (null, "", invalid) is sent to [AdNetworkInitParams](#AdNetworkInitParams) while initializing, **callback** of the listener [IAdInitializationListener.OnInitializationError](#OnInitializationError) with the error **InitializationErrorType.GGAD_CONNECTOR_INITIALIZE_FAILED** will be called.

If there are arguments of [ISDKConnector[]](#ISdkConnector), the process of initialization will be the following: first, the initialization  **AdNetworkSDK** will be called. Only of the initialization completes successfully, the initialization of connectors sent as arguments will be called.

If thew initialization of all connectors completes successfully, the **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete) will be called.

If the initialization of at least one connector fails, the **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete) and
[IAdInitializationListener.OnInitializationWarning](#OnInitializationWarning) will be called.

If the initialization of all connectors fail, but **AdNetworkSDK** is successfully initialized, the **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete) and
[IAdInitializationListener.OnInitializationWarning](#OnInitializationWarning) will be called.

**Declaration**:

```
public static void Initialize(AdNetworkInitParams _adNetworkInitParams, IAdInitializationListener _listener, ISdkConnector[] _otherConnectors = null)
```

where:

|Type |Name| Description|
|---|---|---|
|[AdNetworkInitParams](#AdNetworkInitParams)| _adNetworkInitParams| Parameters of initialization of **Green Grey** ad network|
| [IAdInitializationListener](#IAdInitializationListener) | _listeher_|Implementation of the listener of initialization |
| [ISdkConnector[]](#ISdkConnector) | otherConnectors | Array of implementation of connectors with third-party **SDK**|

## The method Load <a name = "en_load"></a>

The method loads available advertisement from the network and the cache.

The ad type, the implementation of the listener [IAdLoadListener]() and ad **placementId** are sent to the method. The **placementId** is only used for work with connectors in the current version.

The method runs the process of loading advertisements. When the process competes, the **callback** of the listener with the same ad type will be called.

The method saves advertisement data in cache. The cache is cleaned as soon as the advertisement is shown or after the expiration date of the ad.

There is a limit of number of loading that is managed with a server and updated for each loading. The limit for each ad type is counted personally. If a list of ad types is sent to [AdNetworkInitParams](#AdNetworkInitParams) for automatic loading,  **AdNetworkSDK** will support the maximum number of ads in the application cache (**AdNetworkSDK** does not implement the same algorithm for connectors).

If there is at least one connector, the algorithm will be the following:

1. If the ad type is supported with the server, the server tries to load it.
2. If error, **AdNetworkSDK** calls next connector in the list.
3. When connectors complete their work the **callback** of the listener with the same ad type will be called.

**Declaration**:

```
public static void Load(AdType _adType, IAdLoadListener _listener, string _placementId)
```

where:

`AdType` - advertisement type (see [AdType]());

`IAdLoadListener` - implementation of listener of loading (see [IAdLoadListener]());

`string placementId` - advertisement placement.

## The method Show <a name = "en_show"></a>

It shows loaded advertisement.

The ad type, implementation of the listener [IAdShowListener] and advertisement **placementId** are sent to the method. The **placementId** is used only for cooperation with connectors in the current version.

The method runs the process of ad show.

The **callback** [IAdShowListener.OnShowStart](#OnShowStart) is called before show.

The **callback** [IAdShowListener.OnShowComplete](#OnShowComplete) is called if the show completes successfully, The **callback** informs the system about completing status.

This is important for ad type **AdType.REWARDED** for example. Because a user receives award only if the advertisement show completes (**ShowCompletionState.SHOW_COMPLETE_BY_CLOSE_BUTTON**), not skips (**ShowCompletionState.SHOW_COMPLETE_BY_SKIP_BUTTON**).

When the show fails, the **callback** [IAdShowListener.OnShowError](#OnShowError) will be called.
For example, when ad cache expired before call, the error **ShowErrorType.VIDEO_WAS_DELETED** will be sent to the method.

The cache is cleaned every time the show completes successfully or fails.

**Declaration**:

```
public static void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)
```

where:

`AdType` - advertisement type (see [AdType]());

`IAdShowListener` - implementation of listener of show (see [IAdShowListener]());

`string placementId` - advertisement placement.

# Listeners <a name ="en_listener"></a>

Listeners are interfaces that allow to take under control processes of initialization, loading and showing of advertisements.

There are three types of listeners in the system:

• [Listener of initialization IAdInitializationListener](#l_initialization);
• [Listener of loading IAdLoadListener](#l_load);
• [Listener of show IAdShowListener](#l_show).

## Listener of initialization <a name = "en_l_initialization"></a>

An interface of the listener of initialization **SDK** **IAdInitializationListener** is used to take under control the process of initialization.

It uses the following public methods:

• [OnInitializationComplete](#OnInitializationComplete): initialization completion handler;
• [OnInitializationError](#OnInitializationError): initialization error handler;
• [OnInitializationWarning](#OnInitializationWarning): initialization non-critical error handler.

### OnInitializationComplete <a name = "en_OnInitializationComplete"></a>

Initialization completion handler is called when the initialization is completed successfully.

**Declaration**:

```
public void OnInitializationComplete();
```

### OnInitializationWarning <a name = "en_OnInitializationWarning"></a>

Initialization non-critical error handler is called when the initialization is completed successfully but there are errors with user's connectors.

**Declaration**:

```
void OnInitializationWarning(InitializationWarningType _warningType, string _warningMessage)
```

where:

| Type | Name | Description |
|---|---|---|
| InitializationWarningType | warningType | Warning type |
| string | warningMessage | Warning information |

**Warning variants**:

| Value | Description |
|---|---|
| UNKNOWN | Unknown warning|
|NOT_ALL_CONNECTORS_WAS_INITIALIZED | An initialization error of one or several connectors|
| THIRD_PARTY_CONNECTOR_WARNING | Warning from third-party connectors |


### OnInitializationError <a name = "en_OnInitializationError"></a>

Initialization error handler is called when initialization is failed.

**Declaration**:

```
public void OnInitializationError(InitializationErrorType _error, string _errorMessage);
```

| Type | Name | Description |
|---|---|---|
| InitializationErrorType | error | Error type |
| string | errorMessage | Error information |

**Error variants**:

| Value | Description |
|---|---|
| UNKNOWN | Unknown error |
| GAME_ID_IS_NULL_OR_EMPTY | Empty game identifier is specified |
| SDK_ALREADY_INITIALIZED | Initialization has already been done |
| INITIALIZE_PROCESS_ALREADY_STARTED | Initialization has already been started |
| GGAD_CONNECTOR_INITIALIZE_FAILED | Initialization of connectors fails |
| THIRD_PARTY_CONNECTOR_ERROR | Error with user's connector |
| INVALID_GAME_ID | Invalid game identifier |


## Listener of loading

An interface of the listener of loading **IAdLoadListener** is used to take under control the process of loading.

It uses the following public methods:

• [OnLoadComplete](#OnLoadComplete) - loading completion handler;
• [OnLoadError](#OnLoadError) - loading error handler.

### OnLoadComplete <a name ="en_OnLoadComplete"></a>

Loading completion handler is called when loading is completed successfully.

**Declaration**:

```
void OnLoadComplete(AdType _adType) 

````

where: 

| Type | Name | Description |
|---|---|---|
| AdType | AdType | Advertisement type (see [AdType](#adtype)) |


### OnLoadError <a name = "en_OnLoadError"></a>

Loading error handler is called when loading is failed.

**Declaration**:

```
void OnLoadError(AdType _adType, LoadErrorType _error, string _errorMessage)
```

where: 

| Type | Name | Description | 
|---|---|---|
| AdType | AdType | Advertisement type (see [AdType](#adtype))|
| LoadErrorType | error | Error type |
| string  | errorMessage | Error message |

**Error types**:

| Value | Description |
|---|---|
| UNKNOWN | Unknown error |
| CONNECTION_ERROR | Connection error |
| DATA_PROCESSING_ERROR | Data processing error |
| PROTOCOL_ERROR | Protocol error |
| NOT_INITIALIZED_ERROR | Lack of initialization |
| INITIALIZATION_NOT_FINISHED | Initialization is not completed |
| TO_MANY_VIDEOS_LOADED | The upload limit for this video type has been exceeded |
| AVAILABLE_VIDEO_NOT_FOUND | The advertising service did not find the corresponding video |
| NO_CONTENT | No content |
| NOT_SUPPORTED_AD_TYPE | Current type is not supported with the connector |
| THIRD_PARTY_CONNECTOR_ERROR | Error with user's connector |
| REQUEST_NOT_CREATED | Request creation error |
| NO_CONNECTORS_RECEIVED | No valid connectors |
| WEBVIEW_CONTENT_NOT_LOADED | WebView loading error |

## Listener of show <a name = "en_l_show"></a>

An interface of the listener of show **IAdShowListener** is used to take under control the process of show.

It uses the following public methods:

• [OnShowStart](#OnShowStart) - handler of beginning the ad show;
• [OnShowComplete](#OnShowComplete) - handler of completing the ad show;
• [OnShowError](#OnShowError) - show error handler.

### OnShowStart  <a name = "en_OnShowStart"></a>

A handle of beginning the ad show is called before the show starts.

**Declaration**:

```
void OnShowStart(AdType _adType)
```

where:

|Type|Name|Description|
|---|---|---|
|AdType | AdType | Advertisement type (see [AdType](#adtype))

### OnShowComplete <a name = "en_OnShowComplete"></a>

The handler of completing the ad show is call after the show finished.

**Declaration**:

```
void OnShowComplete(AdType _adType, ShowCompletionState _showCompletionState, string _validationId)
```

where:

|Type|Name|Description|
|---|---|---|
|AdType | AdType | Advertisement type (see [AdType](#adtype))
|ShowCompletionState | ShowCompletionState | Status of completing the ad show|
|string | validationId| Identifier of demonstrated  advertisement for server validation|

**Status variants**:

| Value| Description|
|---|---|
|SHOW_COMPLETE_BY_CLOSE_BUTTON| Show completes with the button **Close**|
|SHOW_COMPLETE_BY_SKIP_BUTTON| Show completes with the button **Skip**|

### OnShowError  <a name = "en_OnShowError"></a>

The show error handler is called when the show fails.

**Declaration**:

```
void OnShowError(AdType _adType, ShowErrorType _error, string _errorMessage)
```

where:

| Type| Name| Description|
|---|---|---|
| AdType | AdType | Advertisement type (see [AdType](#adtype))|
| ShowErrorType | error| Error type|
|string | errorMessage| Identifier of demonstrated  advertisement for server validation|

**Error variants**:

| Value| Description| 
|---|---|
|UNKNOWN| Unknown error|
|NOT_INITIALIZED_ERROR| No initialization|
|INITIALIZATION_NOT_FINISHED| Initialization process is not completed|
|VIDEO_PLAYER_ERROR| Player error|
|NO_LOADED_CONTENT| No loaded content|
|THIRD_PARTY_CONNECTOR_ERROR| Error with user's connector|
|CONNECTORS_NOT_RECEIVED| No valid connectors|
|NOT_SUPPORTED_AD_TYPE| Advertisement type is not supported|

# Object models <a name = "en_AdNetworkInitParams"></a>

There is one object model **AdNetworkInitParams** in the advertisement **SDK**, where parameters of initialization of **Green Grey** ad network are collected. 

**Constuctor**:

```
public AdNetworkInitParams(string _gameId, bool _isTestMode, bool _autoLoadEnabled, List<AdType> _adTypesForAutoLoad)
```

**Attributes**:

| Type| Name| Description|
|---|---|---|
|string | | gameId | Application identifier|
|bool | isTestMode| Test mode flag|
| bool | autoLoadEnabled| If automatic loading of advertisements is allowed after initialization|
|List AdType| adTypesForAutoLoad| A list AdType for automatic loading|

# Connectors <a name = "en_ISdkConnector"></a>

An interface of connectors of third-party advertisement **SDK** **ISdkConnector** allows to manage all advertisement **SDK** that are integrated into the application via **AdNetworkSDK**.

It uses the following public methods:

| Method | Description |
|---|---|
|Initialize| Initialization of a connector|
|Load| Loading of advertisements|
|Show| Show of advertisement|
|GetSupportedAdTypes| Receiving a list of supported ad types|
|GetSdkId| Receiving of identifier|

**Initialize** is a method that initializes connector work. Its implementation depends on a user. **Callback** must be returned when it is finished.

**Declaration**:

```
void Initialize(IAdInitializationListener _listener)
```

where:

**IAdInitializationListener** is a listener of initialization (see [IAdInitializationListener](#IAdInitializationListener))

**Load** is a method that loads advertisements. Its implementation depends on a user. **Callback** must be returned when it is finished.

**Declaration**:

```
void Load(AdType _adType, IAdLoadListener _listener, string _placementId = null)
```

where:

**AdType** is an ad type:  <a name = "en_adtype"></a>

|Type| Description|
|---|---|---|
|INTERSTITIAL| Video without donation|
| REWARDED| Video with donation|
|BANNER| Banner (not implemented in the service, added to work with the connector)|
|MREC| MREC (not implemented in the service, added to work with the connector)|

**IAdLoadListener** is a listener of loading (see [IAdLoadListener](#AdLoadListener))

**Show** is a method that shows the advertisement. Its implementation depends on a user. **Callback** must be returned when it is finished.

**Declaration**:

```
void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)
```

where:

**AdType**  is an ad type (see [AdType](#adtype))

**IAdShowListener** is an interface of the listener of show an advertisement (see [IAdShowListener](#AdShowListener)). 

**GetSupportedAdTypes** is a method that returns a list of supported ad types. Its implementation depends on a user.

**Declaration**:
```
List<AdType> GetSupportedAdTypes()
```

**GetSdkID** is a method that returns connector identifier. Its implementation depends on a user.

**Declaration**: 

```
string GetSdkId()
```

The interface of the connector for third-party advertisement **SDK** has the following public attributes:

| Type| Name| Description|
|---|---|---|
|bool| isInitialized| If connector is initialized|
|bool| isInitializedStarted| If initialization is started|\
