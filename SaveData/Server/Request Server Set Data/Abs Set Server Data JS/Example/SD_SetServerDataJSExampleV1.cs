
using System;
using UnityEngine;

public class SD_SetServerDataJSExampleV1 : SD_AbsSetServerDataJS
{
    public override bool IsInit => true;
    public override event Action OnInit;

    [SerializeField] 
    private StatusCallBackServer _statusServer;

    /// <summary>
    /// Если конечно сервер вообще в ответ что то посылает
    /// </summary>
    [SerializeField] 
    private string _returnDataJS;
    
    private void Awake()
    {
        OnInit?.Invoke();
    }

    public override void SetServerDataJS(Action<int, StatusCallBackServer, SD_DataSetRequestServerJSWrapperAddDataJS, string> callback, int id, string addDataJs, string keyInstanceClass, string pushDataJS)
    {
        var data = new SD_DataSetRequestServerJSWrapperAddDataJS(new SD_DataSetRequestServerJS(_returnDataJS), addDataJs);

        if (_statusServer == StatusCallBackServer.Ok)
        {
            // типа данные куда то успешно отправились
            Debug.Log("Данные были успешно отправлены JS = " + pushDataJS);    
        }
        
        callback.Invoke(id, _statusServer, data, keyInstanceClass);
    }
}
