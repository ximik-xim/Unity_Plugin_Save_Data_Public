
using UnityEngine;
/// <summary>
/// Будет содержать данные(JS) получаймые от сервера, в ответ на действие по отправки на этот сервер данных
/// (Нужен будет в случае, если в ответ на отправку данных, сервер отошлет какие то важные данные, пример какойнибудь ключ доступа)
/// </summary>
[System.Serializable]
public class SD_DataSetRequestServerJS 
{
    public SD_DataSetRequestServerJS()
    {

    }

    public SD_DataSetRequestServerJS(string dataJS)
    {
        _dataJS = dataJS;
    }
    
    [SerializeField]
    private string _dataJS;

    public string DataJS => _dataJS;
}