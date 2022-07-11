# Пример работы с sdk
Ниже дан пример кода с объяснениями, который иллюстрирует максимально быстрый в разработке способ показать рекламу.

Создается один компонент `MonoBehaviour`, который при этом реализует все 3 интерфейса слушателей: инициации, загрузки и показа.

Соответствующие функции класса [AdNetworkSDK](../api/methods/AdNetworkSDK.md) вызываются при нажатии кнопок на сцене.

Для загрузки используется функция [Load](../api/methods/Load.md), которая загружает креатив из сети или из кеша если загрузка была проведена ранее.

Данный код можно протестировать на сцене `LoadExampleScene`, поставляющейся в пакете вместе с SDK.

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
---

Не забудьте указать полученный GAME_ID в инспекторе класса.

---

[<< назад](main-principles.md) [далее >>](../api/api.md)