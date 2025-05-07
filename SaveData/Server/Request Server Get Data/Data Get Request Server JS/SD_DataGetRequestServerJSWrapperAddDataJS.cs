using UnityEngine;

/// <summary>
/// Обертка над возращаймыми данные с сервера
/// Нужна, что бы в ответе от сервера получить доп данные JS, которые отправляли на этот сервер
/// </summary>
[System.Serializable]
public class SD_DataGetRequestServerJSWrapperAddDataJS 
{
    public SD_DataGetRequestServerJSWrapperAddDataJS(SD_DataGetRequestServerJS serverDataJS, string addJSData)
    {
        _serverDataJS = serverDataJS;
        _addJSData = addJSData;
    }
    
    public SD_DataGetRequestServerJSWrapperAddDataJS()
    {

    }
    
    /// <summary>
    /// Это данные полученные с сервера
    /// </summary>
    public SD_DataGetRequestServerJS ServerDataJS => _serverDataJS;
    [SerializeField] 
    private SD_DataGetRequestServerJS _serverDataJS;

    /// <summary>
    /// Это те доп. данные JS которые были отправлены на сервер
    /// </summary>
    public string AddJSData => _addJSData;
    [SerializeField]
    private string _addJSData;
}
