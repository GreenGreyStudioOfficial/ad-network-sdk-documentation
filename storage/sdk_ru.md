[Manual](#manual) | [API](#api)

# Принципы работы SDK <a name="manual"></a>

## Оглавление
- [Инициализация библиотеки](#initialization)
- [Загрузка рекламных объявлений](#manual_load)
- [Показ рекламного объявления](#manual_show)
- [Особенности работы системы кэширования](#cache)
- [Как подключить библиотеку](#connect_lib)
- [Как работать с библиотекой - пример](#lib_work)
- [Как интегрироваться со сторонними SDK](#third_party_SDK)
- [Принципы работы с коннектором](#connector)
- [Интеграция](#integration)


Данный SDK используется для работы с рекламой двух видов:

- видеорекламой, воспроизводимой в контексте **Unity**;
- рекламой, отображаемой внутри **web-view**.

Решение о показе какого-либо типа рекламы принимается в зависимости от решений сервера.

| отображается внутри editor | Останавливает основной поток приложения| Останавливает основной поток приложения |
|---|---|---|
| видеореклама | да | нет |
| реклама внутри web-view | нет | да |

Работа с другими типами рекламы реализована через работу коннекторов.

В основе работы **SDK** лежит статический класс **AdNetworkSDK**, который:

- инициирует библиотеку;
- загружает рекламные объявления;
- показывает рекламные объявления.

Для этого в классе имеются открытые статические методы, вызываемые пользователем. Чтобы реагировать на их выполнение, пользователь **SDK** должен самостоятельно реализовать интерфейсы слушателей, в зависимости от своих нужд. Слушатели оповещаются в фоновом режиме. Пример реализации интерфейса слушателя смотрите [здесь](#lib_work).

Данный **SDK** поддерживает интеграцию с рекламными **SDK** других производителей с помощью коннекторов. Пример реализации коннектора смотрите [здесь](#connector).

## Инициализация библиотеки <a name="initialization"></a>

На данном этапе **SDK** подготавливается к загрузке и показу рекламных объявлений, а так же к инициализации коннекторов, реализованных пользователем, для взаимодействия со сторонними рекламными **SDK**.

Пример:

```C#
using System.Collections.Generic;
using GreenGrey.AdNetworkSDK;
using GreenGrey.AdNetworkSDK.DataModel;
using GreenGrey.AdNetworkSDK.DataModel.Enums;
using GreenGrey.AdNetworkSDK.Interfaces.Listeners.Initialization;
using UnityEngine;
using UnityEngine.UI;

namespace GGADSDK.Samples.LoadExample.Scripts
{
    public class InitializeExample: MonoBehaviour, IAdInitializationListener
    {
        [Tooltip("You game id")]
        [SerializeField] private string m_myGameID;
        [Tooltip("Should work in test mode. Dont use true for release buildes")]
        [SerializeField] private bool m_isTestMode;
        [Tooltip("Should sdk auto load ads after success initialization")]
        [SerializeField] private bool m_autoLoadEnabled;
        [Tooltip("Which types can be loaded automatically")]
        [SerializeField] private List<AdType> m_adTypesForAutoLoad;
        [Tooltip("Init button")]
        [SerializeField] private Button m_initButton;

        private void Start()
        {
            m_initButton.onClick.AddListener(InitButtonAction);
        }
        
        private void InitButtonAction()
        {
            Debug.Log("Initialisation started");
            AdNetworkSDK.Initialize(
                new AdNetworkInitParams(
                    m_myGameID,
                    m_isTestMode,
                    m_autoLoadEnabled,
                    m_adTypesForAutoLoad),
                this);
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Initialization: SUCCESS!");
            Debug.Log("Now you can load ads");
        }

        public void OnInitializationError(InitializationErrorType _error, string _errorMessage)
        {
            Debug.LogError($"Initialization failed with error [{_error}]: {_errorMessage}");
        }

        public void OnInitializationWarning(InitializationWarningType _warningType, string _warningMessage)
        {
            Debug.Log($"Warning: {_warningType.ToString()}. {_warningMessage}");
            Debug.Log("Now you can load ads");
        }
    }
}
```

## Загрузка рекламных объявлений <a name="manual_load"></a>

На данном этапе библиотека просит сервис подобрать и скачать подходящую рекламу для данного пользователя. Если рекламный сервис не смог найти рекламу, то он пытается выполнить загрузку через коннекторы пользователя, если они имеются. После того, как успешный ответ получен, рекламное объявление загружается в кэш.

Пример:

```C#
using GreenGrey.AdNetworkSDK;
using GreenGrey.AdNetworkSDK.DataModel.Enums;
using GreenGrey.AdNetworkSDK.Interfaces.Listeners.Load;
using UnityEngine;
using UnityEngine.UI;

namespace GGADSDK.Samples.LoadExample.Scripts
{
    public class LoadExample: MonoBehaviour, IAdLoadListener
    {
        [Tooltip("Load button")]
        [SerializeField] private Button m_loadButton;
        [Tooltip("Ad placementId." +
                 "Required by some ad service, used only for connectors if support" +
                 "Not need for GreenGrey sdk - can be null or empty.")]
        [SerializeField] private string m_placementId;
        private void Start()
        {
            m_loadButton.onClick.AddListener(LoadButtonAction);
        }
        private void LoadButtonAction()
        {
            Debug.Log("Load started");
            AdNetworkSDK.Load(AdType.REWARDED, this, m_placementId);
        }
                public void OnLoadComplete(AdType _adType)
        {
            Debug.Log($"Load [{_adType}]: SUCCESS");
            Debug.Log($"Now you can show {_adType} ad");
        }
        
        public void OnLoadError(AdType _adType, LoadErrorType _error, string _errorMessage)
        {
            Debug.LogError($"Load: failed with error [{_error}]: {_errorMessage}");
        }
    }
}
```

## Показ рекламного объявления <a name="manual_show"></a>

На данном этапе **SDK** показывает пользователю рекламное объявление из кэша. Если загрузка креатива произошла при помощи коннектора, то и показ будет вызван через данный коннектор.

Пример:

```C#
using GreenGrey.AdNetworkSDK;
using GreenGrey.AdNetworkSDK.DataModel.Enums;
using GreenGrey.AdNetworkSDK.Interfaces.Listeners.Show;
using UnityEngine;
using UnityEngine.UI;

namespace GGADSDK.Samples.LoadExample.Scripts
{
    public class ShowExample: MonoBehaviour, IAdShowListener
    {
        [Tooltip("Show button")]
        [SerializeField] private Button m_showButton;
        [Tooltip("Which type you want to show")]
        [SerializeField] private AdType m_adType;
        
        private void Start()
        {
            m_showButton.onClick.AddListener(ShowButtonAction);
        }
        
        private void ShowButtonAction()
        {
            AdNetworkSDK.Show(m_adType, this);
        }
        
        public void OnShowStart(AdType _adType)
        {
            Debug.Log($"Show [{_adType}]: Show started");
        }
        
        public void OnShowComplete(AdType _adType, ShowCompletionState _showCompletionState, string _validationId)
        {
            Debug.Log($"Show [{_adType}]: Show completed with [{_showCompletionState}] complete state\nValidationId: {_validationId}");
            
            // If return _adType == AdType.REWARDED
            // and _showCompletionState == ShowCompletionState.SHOW_COMPLETE_BY_CLOSE_BUTTON
            // you should give user reward
        }

        public void OnShowError(AdType _adType, ShowErrorType _error, string _errorMessage)
        {
            Debug.LogError($"Show [{_adType}]: failed with error [{_error}]: {_errorMessage}");
        }
    }
}
```

### Особенности работы системы кэширования <a name="cache"></a>

Кэширование рекламных объявлений и их очистка происходит автоматически. Пользователь **SDK** не может   влиять на данный процесс.

Очистка кэша может происходить:

- по завершению показа ролика;
- по истечению срока его актуальности. Срок актуальности объявления регулирует рекламодатель.

Особенности работы:

1. Если произошла очистка, попытка воспроизведения загруженного в кэш видеоролика выдает ошибку.

2. Если в **AdNetworkInitParams** передается список типов рекламных объявлений для автозагрузки, то **AdNetworkSDK** поддерживает максимально возможное количество рекламных объявлений в кэше приложения. Данный механизм не реализуется для коннекторов.

3. При повторном вызове **Load** с одним и тем же типом рекламного объявления вызывается **callback** слушателя, если данный тип уже закэширован. Загрузка нового рекламного объявления не происходит.

#### Типы рекламных объявлений

| Тип | Описание |
|---|---|
| `INTERSTITIAL` | Видео без вознаграждения|
| `REWARDED` | Видео с вознаграждением|
| `BANNER` | Баннер (не реализовано в сервисе, добавлен для работы с коннектором) |
| `MREC` | MREC (не реализовано в сервисе, добавлен для работы с коннектором)|

# Как подключить библиотеку <a name="connect_lib"></a>

Библиотека распространяется как пакет для **Package Manager**.

Для работы с библиотекой необходим **GAME_ID** - идентификатор приложения в системе показа рекламы.
Напишите на [a.bobkov@mobidriven.com](a.bobkov@mobidriven.com), чтобы получить идентификатор.

Ссылка на проект в git [https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release](https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release)

Чтобы подключить библиотеку к проекту:

1. В панели **Package Manager** выберите **Add package from git URL**:

![integration_0.png](/images/integration_0.png)

2. В открывшемся окне введите ссылку

[https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release.git#v_N](https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release.git#v_N)

где **N** - текущая версия библиотеки.

3. Для загрузки примера использования в панели **Package Manager** выберите **AdNetworkSDK**, в правой части разверните список примеров и нажмите кнопку **Import**.

![integratiom_1.png](/images/integration_1.png)

После этого рядом с успешно импортированными примерами появится галочка, а сами файлы примеров окажутся в структуре проекта.

![integration_2.png](/images/integration_2.png)

![integration_3.png](/images/integration_3.png)

4. Для запуска примера необходимо прописать полученный идентификатор **GAME_ID** в соответствующем поле редактора:

![integration_4.png](/images/integration_4.png)

# Как работать с библиотекой - пример <a name="lib_work"></a>

Пример кода с объяснениями, который иллюстрирует максимально быстрый в разработке способ показать рекламу.

Создается один компонент **MonoBehaviour**, который при этом реализует все 3 интерфейса слушателей: инициации, загрузки и показа.

Соответствующие функции класса **AdNetworkSDK** вызываются при нажатии кнопок на сцене.
Для загрузки используется функция **Load**, которая загружает рекламное объявление из сети или из кэша, если загрузка была проведена ранее.

Данный код можно протестировать на сцене **LoadExampleScene**, поставляющейся в пакете вместе с **SDK**.

```C#
public class LoadExampleListener : MonoBehaviour, IAdInitializationListener, IAdLoadListener, IAdShowListener  
{  
    [SerializeField] private string m_myGameID;  
    [SerializeField] private Button m_initButton;  
    [SerializeField] private Button m_loadButton;  
    [SerializeField] private Button m_showButton;  
  
    //Last loaded adType  
    private AdType m_adType;
	
    #region MonoBehaviour  
  
    private void Start()  
    {
        m_initButton.onClick.AddListener(InitButtonAction);  
        m_loadButton.onClick.AddListener(LoadButtonAction);  
        m_showButton.onClick.AddListener(ShowButtonAction);  
        m_loadButton.interactable = false;  
        m_showButton.interactable = false;  
    }
	
    private void InitButtonAction()  
    {        
        Debug.Log("Initialisation started");  
        AdNetworkSDK.Initialize(  
            new AdNetworkInitParams(  
                m_myGameID,  
                true,  
                true,  
                new List<AdType>()), this);  
    }
	
    private void LoadButtonAction()  
    {        
        Debug.Log("Load started");  
        m_showButton.interactable = false;  
        AdNetworkSDK.Load(AdType.REWARDED, this, null);  
    }
	
    private void ShowButtonAction()  
    {        
        Debug.Log($"Start showing with type: [{m_adType}]");  
        AdNetworkSDK.Show(m_adType, this);  
    } 
	
    #endregion  
  
    #region IAdInitializationListener  
  
    public void OnInitializationComplete()  
    {        
        Debug.Log("Initialization: SUCCESS!");  
        m_loadButton.interactable = true;  
    }  
    
    public void OnInitializationError(InitializationErrorType _error, string _errorMessage)  
    {        
        Debug.LogError($"Initialization failed with error [{_error}]:{_errorMessage}");  
    }  
    
    public void OnInitializationWarning(InitializationWarningType _warningType, string _warningMessage)  
    {        
        Debug.Log($"Warning: {_warningType.ToString()}. {_warningMessage}");  
    }  
    #endregion  
  
    #region IAdLoadListener  
  
    public void OnLoadComplete(AdType _adType)  
    {        
        m_adType = _adType;  
        m_showButton.interactable = true;  
        Debug.Log($"LazyLoad [{m_adType}]: SUCCESS");  
    }    
	
    public void OnLoadError(AdType _adType, LoadErrorType _error, string _errorMessage)  
    {        
        Debug.LogError($"LazyLoad: failed with error [{_error}]: {_errorMessage}");  
    } 
	
    #endregion  
  
    #region IAdShowListener  
      
    public void OnShowStart(AdType _adType)  
    {        
        Debug.Log($"Show [{_adType}]: Show started");  
        m_showButton.interactable = false;  
    }  
    
    public void OnShowComplete(AdType _adType, ShowCompletionState _showCompletionState, string _validationId)  
    {        
        Debug.Log($"Show [{_adType}]: Show completed with [{_showCompletionState}] complete state\nValidationId: {_validationId}");  
    }  
    
    public void OnShowError(AdType _adType, ShowErrorType _error, string _errorMessage)  
    {        
        Debug.LogError($"Show [{_adType}]: failed with error [{_error}]: {_errorMessage}");  
    }    
	
    #endregion  
}
```

# Как интегрироваться с сторонними рекламными SDK <a name="third_party_SDK"></a>

Для удобства пользователя есть интеграция со сторонними рекламными **SDK**. Интеграция позволяет пользователю использовать несколько рекламных **SDK** на любом этапе работы с проектом.

На данный момент библиотека поддерживает работу с двумя типами рекламы: видеорекламой, воспроизводимой в контексте **Unity**, и рекламой, отображаемой внутри **web-view**. Для работы с остальными типами рекламы необходимо использовать коннекторы.

Коннектор - это механизм, используемый для интеграции со сторонними рекламными **SDK**.

## Принципы работы коннектора <a name="connector"></a>

1. Коннекторы передаются в функцию **Initialize** при инициализации рекламного **SDK**.
2. Во время инициализации рекламного **SDK**, происходит инициализация всех коннекторов. Если какой-то коннектор не прошел инициализацию, выдается предупреждение. Если инициализация рекламного **SDK** не прошла - выдается сообщение об ошибке.
3. Идентификаторы коннекторов передаются на сервер для составления списка приоритетов. 
4. Сервер по порядку опрашивает рекламные **SDK**.
5. Функция **Load** вызывается по порядку по всем коннекторам для загрузки рекламного объявления.
6. Функция **Show** показывает рекламное объявление коннектора, который первым загрузил рекламу.

## Интеграция <a name="integration"></a>

Описание интерфейса коннектора для интеграции со сторонним рекламным **SDK**:

```C#
public interface ISdkConnector
{
    bool isInitialized { get; }

    bool isInitializeStarted { get; }

    void Initialize(IAdInitializationListener _listener);

    void Load(AdType _adType, IAdLoadListener _listener, string _placementId = null);

    void Show(AdType _adType, IAdShowListener _listener, string _placementId = null);

    List<AdType> GetSupportedAdTypes();

    string GetSdkId();
}
```

| Функция | Описание |
|---|---|
| `isInitialized` | Свойство "закончена ли инициализация самого **SDK**" |
| `isInitializeStarted` | Свойство "начата ли инициализация самого **SDK**" |
| `Initialize(IAdInitializationListener _listener)` | Инициализация коннектора. В качестве аргумента указывается слушатель
| `Load(AdType _adType, IAdLoadListener _listener, string _placementId = null)` | Загрузка рекламы коннектором. В качестве аргументов указывается тип рекламы, слушатель загрузки и **placementId** - если есть|
| `Show(AdType _adType, IAdShowListener _listener, string _placementId = null)` | Показ рекламы. А качестве аргументов указывается тип рекламы, слушатель показа и **placementId** - если есть |
| `GetSupportedAdTypes` | Типы рекламы, поддерживаемые сторонним рекламным **SDK** |
| `GetSdkId` | Идентификатор стороннего рекламного **SDK**. Используется для подключения **SDK** через администраторов **GreenGrey Studio**|
****
При подключении библиотеки загружается пример реализации коннектора **Applovin**.

















<br/><br/>
<br/><br/>
[Manual](#manual) | [API](#api)
_____
# API <a name="api"></a>

# Статичный класс AdNetworkSDK
Статичный класс **AdNetworkSDK** - публичный интерфейс для взаимодействия с **SDK**.

Включает в себя следующие публичные методы:

- [Initialize](#initialize): инициализации работы **SDK**;
- [Load](#api_load): загрузка доступного рекламного объявления из сети или из кэша;
- [Show](#api_show): показ загруженного рекламного объявления.

## Содержание

- [Метод Initialize](#initialize)
- [Метод Load](#api_load)
- [Метод Show](#api_show)
- [Слушатели](#listeners)
- [Слушатель инициализации](#l_initialization)
- [Слушатель загрузки](#l_load)
- [Слушатель показа](#l_show)
- [Модель объекта AdNetworkInitParams](#AdNetworkInitParams)
- [Коннекторы ISdkConnector](#ISdkConnector)

## Метод Initialize <a name = "initialize"></a>

Метод **Initialize** инициализирует работу **SDK**.

На вход передаются параметры инициализации **SDK** [AdNetworkInitParams](#AdNetworkInitParams), реализация слушателя [IAdInitializationListener](#IAdInitializationListener) и массив коннекторов, реализующих интерфейс [ISDKConnector](#ISDKConnector) для взаимодействия со сторонними рекламными **SDK**.

Без инициализации методы [AdNetworkSDK.Load](#api_load) и [AdNetworkSDK.Show](#api_show) не отработают корректно и будут сообщать своим слушателям об ошибках
**[LoadErrorType.NOT_INITIALIZED_ERROR](#errors_explanation)** и **[ShowErrorType.NOT_INITIALIZED_ERROR](#errors_explanation)** соответственно.

Если инициализация запущена, но не завершена, методы [AdNetworkSDK.Load](#api_load) и [AdNetworkSDK.Show](#api_show) не отработают корректно и будут сообщать своим слушателям об ошибках
**[LoadErrorType.INITIALIZATION_NOT_FINISHED](#errors_explanation)** и **[ShowErrorType.INITIALIZATION_NOT_FINISHED](#errors_explanation)**
соответственно.

Если **SDK** успешно инициализирован, то при попытке повторной инициализации, вызовется **callback** ее слушателя [IAdInitializationListener.OnInitializationError](#OnInitializationError) с ошибкой **[InitializationErrorType.SDK_ALREADY_INITIALIZED](#errors_explanation)**.

Если **SDK** в процессе инициализации, то при попытке повторной инициализации, вызовется **callback** ее слушателя [IAdInitializationListener.OnInitializationError](#OnInitializationError) с ошибкой **[InitializationErrorType.INITIALIZE_PROCESS_ALREADY_STARTED](#errors_explanation)**.

Если при инициализации в [AdNetworkInitParams](#AdNetworkInitParams) был передан некорректный **GAME_ID** (null, "", invalid), то будет вызван **callback** слушателя [IAdInitializationListener.OnInitializationError](#OnInitializationError) с ошибкой **[InitializationErrorType.GGAD_CONNECTOR_INITIALIZE_FAILED](#errors_explanation)**.

В случае наличия аргументов [ISDKConnector[]](#ISDKConnector) процесс инициализации выстраивается следующим образом: сначала вызывается инициализация **AdNetworkSDK** и только в случае успешной инициализации вызывается инициализация коннекторов переданных в аргументах.

Если инициализация всех коннекторов прошла успешно, то вызывается **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete).

Если инициализация хотя бы одного коннектора провалилась, то вызывается **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete) и
[IAdInitializationListener.OnInitializationWarning](#OnInitializationWarning).

Если инициализация всех коннекторов провалилась, но **AdNetworkSDK** был успешно проинициализирован, то также будет вызван **callback** [IAdInitializationListener.OnInitializationComplete](#OnInitializationComplete) и
[IAdInitializationListener.OnInitializationWarning](#OnInitializationWarning).

**Объявление**:

```C#
public static void Initialize(AdNetworkInitParams _adNetworkInitParams, IAdInitializationListener _listener, ISdkConnector[] _otherConnectors = null)
```

где:

| Тип| Имя| Описание|
|---|---|---|
|[AdNetworkInitParams](#AdNetworkInitParams)| adNetworkInitParams| Параметры инициализации рекламной сети **Green Grey**|
|[IAdInitializationListener](#IAdInitializationListener)| listener| Реализация слушателя инициализации|
| [ISdkConnector[]](#ISdkConnector) | otherConnectors | Массив реализаций коннекторов со сторонними **SDK**|

## Метод Load <a name = "api_load"></a>

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

```C#
public static void Load(AdType _adType, IAdLoadListener _listener, string _placementId)
```

где:

`AdType` - тип рекламного объявления (см. [AdType](#adtype));

`IAdLoadListener` - реализация слушателя загрузки (см. [IAdLoadListener](#IAdLoadListener));

`string _placementId` - плейсмент рекламного объявления.

## Метод Show <a name = "api_show"></a>

Показывает загруженное рекламное объявление.

На вход нужно передать [тип рекламного объявления](#adtype), реализацию слушателя [IAdShowListener](#l_show) и **placementId** рекламного креатива. В текущей версии **placementId** используется только для работы с коннекторами.

Метод запускает процесс показа ролика.

Перед показом вызывается **callback** [IAdShowListener.OnShowStart](#OnShowStart)

По окончанию успешного показа у слушателя вызовется **callback** [IAdShowListener.OnShowComplete](#OnShowComplete), сообщив о статусе завершения.

Это важно, если, например, объявление имело тип `AdType.REWARDED`. Так как в этом случае для присуждения награды нужно знать был ли ролик досмотрен до конца (`ShowCompletionState.SHOW_COMPLETE_BY_CLOSE_BUTTON`) или пропущен (`ShowCompletionState.SHOW_COMPLETE_BY_SKIP_BUTTON`).

В случае, если во время показа произошла ошибка, будет вызван **callback** [IAdShowListener.OnShowError](#OnShowError).
Так, например, если срок кэша ролика истек до вызова, то на вход методу передастся ошибка `ShowErrorType.VIDEO_WAS_DELETED`.

После завершения показа, вне зависимости от его успешности, кэш объявления очищается.

**Объявление**:

```C#
public static void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)
```

где:

`AdType` - тип рекламного объявления (см. [AdType](#adtype));

`IAdShowListener` - реализация слушателя показа (см. [IAdShowListener](#l_show));

`string _placementId` - плейсмент рекламного объявления.

# Слушатели <a name = "listeners"></a>

Слушатели (или **listeners**) - это интерфейсы, которые дают возможность контролировать процессы инициализации, загрузки и показа рекламных объявлений.

В системе используется три вида слушателей:

- [Слушатель инициализации IAdInitializationListener](#l_initialization);
- [Слушатель загрузки IAdLoadListener](#l_load);
- [Слушатель показа IAdShowListener](#l_show).

## Слушатель инициализации <a name = "l_initialization"></a>

Интерфейс слушателя инициализации **SDK** **IAdInitializationListener** используется для контроля выполнения процесса инициализации.

Использует следующие публичные методы:

- [OnInitializationComplete](#OnInitializationComplete): обработчик завершения инициализации;
- [OnInitializationError](#OnInitializationError): обработчик ошибок инициализации;
- [OnInitializationWarning](#OnInitializationWarning): обработчик некритических ошибок инициализации.

### OnInitializationComplete <a name = "OnInitializationComplete"></a>

Обработчик завершения инициализации вызывается, когда инициализация прошла успешно.

**Объявление**:

```C#
public void OnInitializationComplete();
```

### OnInitializationWarning <a name = "OnInitializationWarning"></a>

Обработчик некритических ошибок завершения инициализации вызывается, когда инициализация прошла успешно, но есть ошибки в пользовательских коннекторах.

**Объявление**:

```C#
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

### OnInitializationError <a name = "OnInitializationError"></a>

Обработчик ошибок завершения инициализации вызывается, когда инициализация прошла с ошибкой.

**Объявление**:

```C#
public void OnInitializationError(InitializationErrorType _error, string _errorMessage);
```

| Тип| Имя| Описание|
|---| --- |---|
|InitializationErrorType| error| Тип ошибки|
|string| errorMessage| Информация об ошибке|

**Варианты ошибок**: <a name = "errors_explanation"></a>

| Значение| Описание| 
|---|---|
| UNKNOWN| Неизвестная ошибка|
|GAME_ID_IS_NULL_OR_EMPTY | Задан пустой идентификатор игры| 
| SDK_ALREADY_INITIALIZED| Инициализация уже была проведена|
| INITIALIZE_PROCESS_ALREADY_STARTED| Процесс инициализации уже запущен| 
| GGAD_CONNECTOR_INITIALIZE_FAILED| Инициализация коннекторов провалилась|
| THIRD_PARTY_CONNECTOR_ERROR| Ошибка в пользовательском коннекторе| 
| INVALID_GAME_ID| Недействительный идентификатор игры|

## Слушатель загрузки <a name = "l_load"></a>

Интерфейс слушателя загрузки **IAdLoadListener** используется для контроля выполнения процесса загрузки.

Использует следующие публичные методы:

- [OnLoadComplete](#OnLoadComplete) - обработчик завершения загрузки;
- [OnLoadError](#OnLoadError) - обработчик ошибок загрузки рекламных объявлений.

### OnLoadComplete <a name = "OnLoadComplete"></a> 

Обработчик завершения загрузки вызывается, когда загрузка прошла успешно.

**Объявление**:

```C#
void OnLoadComplete(AdType _adType) 
```

где: 

| Тип| Имя| Описание|
|---|---|---|
| AdType| AdType| Тип рекламного объявления (см. [AdType](#adtype))|

### OnLoadError <a name = "OnLoadError"></a> 

Обработчик ошибок загрузки вызывается, когда загрузка прошла с ошибкой.

**Объявление**:

```C#
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


## Слушатель показа <a name = "l_show"></a>

Интерфейс слушателя показа рекламного объявления **IAdShowListener** используется для контроля выполнения процесса показа.

Поддерживает следующие публичные методы:

- [OnShowStart](#OnShowStart) - обработчик начала показа рекламного объявления;
- [OnShowComplete](#OnShowComplete) - обработчик завершения показа объявления;
- [OnShowError](#OnShowError) - обработчик ошибок показа рекламного объявления.

### OnShowStart <a name = "OnShowStart"></a> 

Обработчик начала показа рекламного объявления вызывается перед началом показа.

**Объявление**:

```C#
void OnShowStart(AdType _adType)
```

где:

|Тип| Имя| Описание|
|---|---|---|
|AdType | AdType | Тип рекламного объявления(см. [AdType)](#adtype)|

### OnShowComplete <a name = "OnShowComplete"></a> 

Обработчик завершения пока рекламного объявления вызывается, когда показ прошел успешно.

**Объявление**:

```C#
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

### OnShowError <a name = "OnShowError"></a>  

Обработчик ошибок показа рекламного объявления вызывается, когда показ прошел с ошибкой.

**Объявление**:

```C#
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

# Модели объектов <a name = "AdNetworkInitParams"></a>  

В рекламной **SDK** представлена одна модель объектов **AdNetworkInitParams**, в которой собраны параметры инициализации рекламной сети **Green Grey**. 

**Конструктор**:

```C#
public AdNetworkInitParams(string _gameId, bool _isTestMode, bool _autoLoadEnabled, List<AdType> _adTypesForAutoLoad)
```

**Свойства**:

| Тип| Имя| Описание|
|---|---|---|
| string | gameId| Идентификатор приложения| 
| bool | isTestMode| Флаг тестового режима|
| bool | autoLoadEnabled | Разрешена ли автоматическая загрузка рекламных объявлений после инициализации|
| List AdType | adTypesForAutoLoad| Список AdType для автоматической загрузки|

# Коннекторы <a name = "ISdkConnector"></a>

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

```C#
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

`AdType _adType` - тип рекламного объявления:  <a name = "adtype"></a>

| Значение| Описание| 
|---|---|
| INTERSTITIAL| Видео без вознаграждения| 
|REWARDED| Видео с вознаграждением|
| BANNER| Баннер (не реализовано в сервисе, добавлен для работы с коннекторами)| 
|MREC| MREC (не реализовано в сервисе, добавлен для работы с коннекторами)|

`IAdLoadListener _listener` - слушатель загрузки ( см. [IAdLoadListener](#IAdLoadListener))

**Show** - метод, который показывает рекламное объявление. Реализация зависит от пользователя. По завершении должен вернуться **callback**.

**Объявление**:

```C#
void Show(AdType _adType, IAdShowListener _listener, string _placementId = null)
```

где:

`AdType _adType` - тип рекламного объявления (см. [AdType](#adtype))
`IAdShowListener _listener` - интерфейс слушателя показа рекламного объявления (см. [IAdShowListener](#l_show)). 

**GetSupportedAdTypes** - метод, который возвращает список поддерживаемых типов. Реализация зависит от пользователя.

**Объявление**:

```C#
List<AdType> GetSupportedAdTypes()
```

**GetSdkID** - метод, который возвращает идентификатор коннектора. Реализация зависит от пользователя.

**Объявление**: 

```C#
string GetSdkId()
```

Интерфейс коннектора сторонних рекламных **SDK** имеет следующие публичные свойства:

| Тип| Имя| Описание|
|---|---|---|
| bool| isInitialized| Инициализирован ли коннектор|
| bool| isInitializedStarted| Начат ли процесс инициализации|





