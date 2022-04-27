# Быстрый способ показать рекламу
Ниже дан пример кода с объяснениями, который иллюстрирует максимально быстрый в разработке способ показать рекламу.

Создается один компонент `MonoBehaviour`, который при этом реализует все 3 интерфейса слушателей: инициации, загрузки и показа.

Соответствующие функции класса [AdNetworkSDK](../api/methods/AdNetworkSDK.md) вызываются при нажатии кнопок на сцене.

Для загрузки используется функция [LazyLoad](../api/methods/LazyLoad.md), которая позволяет избежать кэширования лишних креативов, а значит и обработку ошибок, связанных с этим.

Данный код можно протестировать на сцене `LazyLoadExampleScene`, поставляющейся в пакете вместе с SDK.

```
public class LazyLoadExampleListener : MonoBehaviour, 
	IAdInitializationListener, IAdLoadListener, IAdShowListener
{
    //ID приложения пользователя, полученное на сайте сервиса
    private const string MY_GAME_ID = "MY_GAME_ID";
    //Адрес хоста сервиса, полученный на сайте сервиса
    private const string AD_SERVER_HOST = "AD_SERVER_HOST";
    
    [SerializeField] private Button m_initButton; //кнопка инициализации
    [SerializeField] private Button m_loadButton; //кнопка загрузки
    [SerializeField] private Button m_showButton; //кнопка показа

    private string m_loadedId; //переменная для сохранения ID скаченной рекламы
    
    #region MonoBehaviour

    private void Start()
    {
        // Назначаем действия кнопкам
        m_initButton.onClick.AddListener(InitButtonAction);
        m_loadButton.onClick.AddListener(LoadButtonAction);
        m_showButton.onClick.AddListener(ShowButtonAction);
    }

    // Действие кнопки инициализации.
    private void InitButtonAction()
    {
        Debug.Log("Initialisation started");
        AdNetworkSDK.Initialize(MY_GAME_ID, AD_SERVER_HOST, true, this);
    }

    // Действие кнопки загрузки.
    private void LoadButtonAction()
    {
        Debug.Log("LazyLoad started");
        AdNetworkSDK.LazyLoad(AdType.REWARDED, this);
    }
    
    // Действие кнопки показа.
    private void ShowButtonAction()
    {
        Debug.Log($"Start showing with id: [{m_loadedId}]");
        AdNetworkSDK.Show(m_loadedId, this);
    }

    #endregion

    #region IAdInitializationListener

    // Действие при успешной инициализации
    public void OnInitializationComplete()
    {
        Debug.Log("Initialization: SUCCESS!");
    }

    // Действие, если инициализация прошла с ошибками
    public void OnInitializationError(InitializationErrorType _error, string _errorMessage)
    {
        Debug.LogError($"Initialization failed with error [{_error}]: {_errorMessage}");
    }

    #endregion

    #region IAdLoadListener

    // Действие при успешной загрузке
    public void OnLoadComplete(string _id)
    {
        m_loadedId = _id;
        Debug.Log($"LazyLoad [{_id}]: SUCCESS");
    }
    
    // Действие, если загрузка прошла с ошибкой
    public void OnLoadError(LoadErrorType _error, string _id, string _errorMessage)
    {
        Debug.LogError($"LazyLoad [{_id}]: failed with error [{_error}]: {_errorMessage}");
    }

    #endregion

    #region IAdShowListener
    
    // Действие перед началом показа
    public void OnShowStart(string _id)
    {
        Debug.Log($"Show [{_id}]: Show started");
    }

    // Действие при завершении показа
    public void OnShowComplete(string _id, ShowCompletionState _showCompletionState)
    {
        Debug.Log($"Show [{_id}]: Show completed with [{_showCompletionState}] complete state");
    }
    
    // Действие при возникновении ошибки во время показа
    public void OnShowError(string _id, ShowErrorType _error, string _errorMessage)
    {
        Debug.LogError($"Show [{_id}]: failed with error [{_error}]: {_errorMessage}");
    }
    
    #endregion
}
```
---

Главный плюс этого метода - он прост и быстр в реализации. Но зачем тогда нужно вообще контролировать кэш, если все так хорошо?

И тут на сцену выходит главный минус данного метода, который отвечает на выше поставленный вопрос: загружая креативы с помощью [LazyLoad](../api/methods/LazyLoad.md) мы никогда не сможем закэшировать несколько рекламных объявлений (например, чтобы сэкономить время в будущем, или иметь запас на случай отсутствия интернета).

Далее мы расскажем, как можно обойти этот минус данного метода.

---

[<< назад](main-principles.md) [далее >>](cached-show.md)