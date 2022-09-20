[Manual](#manual) | [API](#api)

# SDK work way <a name="manual"></a>
## Contents
- [Initialization of the libraty](#initialization)
- [Loading of the advertisements](#load)
- [Show of the advertisement](#show)
- [Cache system features](#cache)
- [How to connect the library](#connect_lib)
- [How to work with the library. Example](#lib_work)
- [How to integrate third-party SDK](#third_party_SDK)
- [Connector work way](#connector)
- [Integration](#integration)

This **SDK** is used for work with the advertisements of two types:

• video advertisement playing in the context of **Unity**;
• advertisement shown inside web view.

Depending the server decisions one of the ad type with be shown.


| | Shown inside editor | Stop application stream |
|---|---|---|
| video advertisement | yes | no |
| advertisement inside web view | no | yes |

Work with other types of the advertisement is implemented via connectors work.

At the heart of **SDK** work there is a statistic class **AdNetworkSDK** that:

• initialize the library;
• load an advertisement;
• show an advertisement.

For this, there are open-source statistic methods in the class. These methods are called by users. To cooperate with them, a **SDK** user must develop listener interfaces by him own depending on his needs. Listeners are notified in the background. An example of listener interface see [here](#lib_work).

**SDK** supports integration with third-party advertisement **SDK** via connectors. An example of connector see [here](third_party_SDK).

## Initialization of the library <a name="initialization"></a>

**SDK** is prepared for loading and showing of the advertisements. Connectors that are implemented by users for interaction with the third-party advertisement **SDK** are also initialized here.

Example:

```
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

## Loading of the advertisements <a name="load"></a>

The library connects to the server to select and load the appropriated advertisement for the current user. If the ad server can find no advertisement, it tries to use the user's connectors for loading (if the user implement them). When the successful answer is received, the advertisement is loaded in the cache.

Example:

```
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

## Show of the advertisement <a name="show"></a>

**SDK** shows the advertisement from the cache. If a connector is used for loading the advertisement, the ad will be shown via the connector.

Example:

```
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

### Cache system features <a name="cache"></a>

Cache of the advertisements and their cleaning are done automatically. **SDK**  user cannot influence on the process.

Cache cleaning can be:

• when show of the video is finished;
• after the expiration of video relevance. The validity period of the advertisement is regulated by the advertiser.

Features:

1. If cache is cleaned, there will be an error while attempting to play a video loaded in the cache.

2. If a list of the advertisement type in loaded to **AdNetworkInitParams** for automatic load, **AdNetworkSDK** supports the maximum number of advertisements in the application cache. This feature is not implemented for connectors.

3. When the function **Load** is called repeatedly for the same advertisement type, the listener **callback** is called if the type is already added to the cache. A new advertisement is not loaded.

#### Advertisement types

| Type | Description |
|---|---|
| `INTERSTITIAL` | Video without donation |
| `REWARDED` | Video with donation |
| `BANNER` | Banner (not implemented in the service, added to work with the connector)|
| `MREC` | MREC (not implemented in the service, added to work with the connector) |


## How to connect the library <a name="connect_lib"></a>

The library represents a package for **Package Manager**.

For work with the library you need **GAME_ID** - an application identifier in the system of showing advertisements.

Connect [a.bobkov@mobidriven.com](a.bobkov@mobidriven.com) to receive the identifier.

Project link in git [https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release](https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release)

To connect the library to the progect:

1. Select **Add package from git URL** in the panel **Package Manager** :

![integration_0.png]("C:\Users\79037\Documents\GG\AdSDK_images\integration_0.png")

2. In the opened window enter the link:

[https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release.git#v_N](https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release.git#v_N)

where **N** is current version of the library.

3. For loading an example select **AdNetworkSDK**  in the panel **Package Manager**.  Expand a list of examples on the right and click **Import**.

![integration_1.png]("C:\Users\79037\Documents\GG\AdSDK_images\integration_1.png")

After that, imported examples will be marked with a tick. Examples files will be added to the structure of the project. 

![integration_2.png]("C:\Users\79037\Documents\GG\AdSDK_images\integration_2.png")

![integration_3.png]("C:\Users\79037\Documents\GG\AdSDK_images\integration_3.png")

4. To run an example, specify the identifier **GAME_ID** in the editor field:

![integration_4.png]("C:\Users\79037\Documents\GG\AdSDK_images\integration_4.png")

## How to work with the library. Example <a name="lib_work"></a>

Example of a code illustrates the fastest way of development how to show advertisement.

A component  **MonoBehaviour** is created. The component implements three listener interfaces: of initialization, of loading, of show.

The functions of the class **AdNetworkSDK**  are called while clicking the scene.

The function **Load** is used for loading. It loads an advertisement from the network or from the cache (if loading was done previously).

The code can be tested on the scene **LoadExampleScene** included into the **SDK** package.

```
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

## How to integrate third-party SDK <a name="third_party_SDK"></a>

For users' convenience there is integration with third-party advertisement **SDK**. The integration allows to use several advertisement **SDK** at any stage of the project.

Now the library supports work with two advertisement types: video advertisement playing in the context of **Unity**, and advertisement shown inside web view. For work with other advertisement types you need to use connectors.

Connector is a feature that is used for integration with third-party advertisement **SDK**.

### Connector work way <a name="connector"></a>

1. Connectors are sent to the function Initialize while initialization of the advertisement **SDK**.

2. While the advertisement **SDK** is being initialized, all the connectors are initialized. If the initialization of one of the connectors is failed, there will be a warning message. If the initialization of the advertisement **SDK** is failed. there will be an error message.

3. Identifiers of the connectors are sent to the server for creating a priority list. 

4. The server sends requests to the advertisement **SDK** seriatim.

5. The function **Load** is called for all the connectors to load the advertisement seriatim.

6. The function **Show** demonstrates advertisement of the connector that was the first which has loaded it.

### Integration<a name="integrartion"></a>

Description of a connector interface for integration with third-party advertisement **SDK**:

```
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

| Function | Description |
|---|---|
| isInitialized | Attribute "**SDK** initialization is finished" |
| isInitializeStarted | Attribute "**SDK** initialization is started"|
|  Initialize(IAdInitializationListener_listener) | Connectors initialization. Argument: listener |
|Load(AdType_adType, IAdLoadListener_listener, string_placementId = null) | Loading of advertisement with the connector. Arguments: advertisement type, loading listener, **placementId** (if it is).|
| Show(AdType_adType, IAdShowListener_listener, string_placementId = null)   | Show of advertisement. Arguments: advertisement type, view listener, **placementId** (if it is).  |
|GetSupportedAdTypes | Advertisement type supported with third-party advertisement **SDK** |
|  GetSdkId | Identifier of third-party advertisement **SDK**. User for connecting **SDK** by **GreenGrey Studio** administrators |


While connecting the library an example of **Applovin** connector is loaded.
















<br/><br/>
<br/><br/>
_____
# API <a name="api"></a>

# Static class AdNetworkSDK

Static class **AdNetworkSDK**  is a public interface for cooperation with **SDK**.

It contains the following public methods:

• [Initialize](): initialization of **SDK** work;
• [Load](): loading of available advertisements from network and chache;
• [Show](): show of loaded advertisement.

## Contents
- [The method Initialize](#initialize)
- [The method Load](#load)
- [The method Show](#show)
- [Listeners](#listeners)
- [Listener of initialization](#l_initialization)
- [Listener of loading](#l_load)
- [Listener of show](#l_show)
- [Object model AdNetworkInitParams](#AdNetworkInitParams)
- [Connectors ISdkConnector](#ISdkConnector)

## The method Initialize <a name ="initialize"></a>

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

## The method Load <a name = "load"></a>

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

## The method Show <a name = "show"></a>

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

# Listeners <a name ="listener"></a>

Listeners are interfaces that allow to take under control processes of initialization, loading and showing of advertisements.

There are three types of listeners in the system:

• [Listener of initialization IAdInitializationListener](#l_initialization);
• [Listener of loading IAdLoadListener](#l_load);
• [Listener of show IAdShowListener](#l_show).

## Listener of initialization <a name = "l_initialization"></a>

An interface of the listener of initialization **SDK** **IAdInitializationListener** is used to take under control the process of initialization.

It uses the following public methods:

• [OnInitializationComplete](#OnInitializationComplete): initialization completion handler;
• [OnInitializationError](#OnInitializationError): initialization error handler;
• [OnInitializationWarning](#OnInitializationWarning): initialization non-critical error handler.

### OnInitializationComplete <a name = "OnInitializationComplete"></a>

Initialization completion handler is called when the initialization is completed successfully.

**Declaration**:

```
public void OnInitializationComplete();
```

### OnInitializationWarning <a name = "OnInitializationWarning"></a>

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


### OnInitializationError <a name = "OnInitializationError"></a>

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

### OnLoadComplete <a name ="OnLoadComplete"></a>

Loading completion handler is called when loading is completed successfully.

**Declaration**:

```
void OnLoadComplete(AdType _adType) 

````

where: 

| Type | Name | Description |
|---|---|---|
| AdType | AdType | Advertisement type (see [AdType](#adtype)) |


### OnLoadError <a name = "OnLoadError"></a>

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

## Listener of show <a name = "l_show"></a>

An interface of the listener of show **IAdShowListener** is used to take under control the process of show.

It uses the following public methods:

• [OnShowStart](#OnShowStart) - handler of beginning the ad show;
• [OnShowComplete](#OnShowComplete) - handler of completing the ad show;
• [OnShowError](#OnShowError) - show error handler.

### OnShowStart  <a name = "OnShowStart"></a>

A handle of beginning the ad show is called before the show starts.

**Declaration**:

```
void OnShowStart(AdType _adType)
```

where:

|Type|Name|Description|
|---|---|---|
|AdType | AdType | Advertisement type (see [AdType](#adtype))

### OnShowComplete <a name = "OnShowComplete"></a>

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

### OnShowError  <a name = "OnShowError"></a>

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

# Object models <a name = "AdNetworkInitParams"></a>

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

# Connectors <a name = "ISdkConnector"></a>

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

**AdType** is an ad type:  <a name = "adtype"></a>

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
