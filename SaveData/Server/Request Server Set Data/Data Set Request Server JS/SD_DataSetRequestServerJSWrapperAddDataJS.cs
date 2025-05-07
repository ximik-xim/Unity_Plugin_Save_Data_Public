using UnityEngine;

/// <summary>
/// Обертка над данными об ответе сервера при отправке на него данных
/// Нужна, что бы в ответе от сервера получить доп данные JS, которые отправляли на этот сервер
/// </summary>
[System.Serializable]
public class SD_DataSetRequestServerJSWrapperAddDataJS
{
    public SD_DataSetRequestServerJSWrapperAddDataJS(SD_DataSetRequestServerJS serverDataJS, string addJSData)
    {
        _serverDataJS = serverDataJS;
        _addJSData = addJSData;
    }
    
    public SD_DataSetRequestServerJSWrapperAddDataJS()
    {

    }
    
    /// <summary>
    /// Это данные полученные с сервера
    /// </summary>
    public SD_DataSetRequestServerJS ServerDataJS => _serverDataJS;
    [SerializeField] 
    private SD_DataSetRequestServerJS _serverDataJS;

    /// <summary>
    /// Это те доп. данные JS которые были отправлены на сервер
    /// </summary>
    public string AddJSData => _addJSData;
    [SerializeField]
    private string _addJSData;
}
