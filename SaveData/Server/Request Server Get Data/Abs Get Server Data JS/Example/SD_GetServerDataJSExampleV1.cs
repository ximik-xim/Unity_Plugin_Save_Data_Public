using System;
using UnityEngine;

public class SD_GetServerDataJSExampleV1 : SD_AbsGetServerDataJS
{
    public override bool IsInit => true;
    public override event Action OnInit;

    [SerializeField] 
    private StatusCallBackServer _statusServer;

    [SerializeField] 
    private string _returnDataJS;
    
    private void Awake()
    {
        OnInit?.Invoke();
    }

    public override void GetServerDataJS(Action<int, StatusCallBackServer, SD_DataGetRequestServerJSWrapperAddDataJS, string> callback, int id, string addDataJs, string keyInstanceClass)
    {
        var data = new SD_DataGetRequestServerJSWrapperAddDataJS(new SD_DataGetRequestServerJS(_returnDataJS), addDataJs);
        
        callback.Invoke(id, _statusServer, data, keyInstanceClass);
    }
}
