using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Будет содержать данные(JS) полученные от сервера
/// </summary>
[System.Serializable]
public class SD_DataGetRequestServerJS 
{
    public SD_DataGetRequestServerJS()
    {

    }

    public SD_DataGetRequestServerJS(string dataJS)
    {
        _dataJS = dataJS;
    }
    
    [SerializeField]
    private string _dataJS;

    public string DataJS => _dataJS;
}
