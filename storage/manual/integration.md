## Интеграция в проект
Библиотека распространяется как пакет для Package Manager.

Текущая версия: v1.1.0

## 0. Что понадобится для интеграции?

GAME_ID - идентификатор приложения в системе показа рекламы

AD_SERVER_HOST - имя сервера рекламы

Их Вы можете получить у Александра Бобкова

Ссылка на проект в git [https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release](https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release)

## 1. Добавление в проект

1.1 В панели Package Manager выберите Add package from git URL

![](../../images/interation_0.png) 

1.2 В открывшемся окне введите ссылку

[https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release.git#v1.1.0](https://github.com/GreenGreyStudioOfficial/AdNetworkSDK_release.git#v1.1.0)

1.3 Для загрузки примера использования нужно в панели Package Manager выбрать AdNetworkSDK, в правой части развернуть список примеров и нажать кнопку “Import”

![](../../images/intergation_1.png)

После этого рядом с успешно импортированными примерами появится галочка, а сами файлы примеров окажутся в структуре проекта

![](../../images/intergation_2.png) | ![](../../images/intergation_3.png)
-|-

1.4 Для запуска примера необходимо прописать имеющиеся идентификаторы GAME_ID и AD_SERVER_HOST в верхней части файла LazyLoadExampleListener:

```
namespace GGADSDK.Samples.LazyLoadExample.Scripts
{
    public class LazyLoadExampleListener : MonoBehaviour, 
		IAdInitializationListener, IAdLoadListener, IAdShowListener
    {
        private const string MY_GAME_ID = "MY_GAME_ID";
        private const string AD_SERVER_HOST = "AD_SERVER_HOST";
```

---

[Далее >>](main-principles.md)