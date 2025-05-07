using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;


#if UNITY_EDITOR
public class SD_GetServerDataJSExampleV2 : SD_AbsGetServerDataJS
{
    [SerializeField] 
    private string _keyInstanceClass = "";
    private static Dictionary<string, SD_GetServerDataJSExampleV2> _instancesClasses = new Dictionary<string, SD_GetServerDataJSExampleV2>();
    private Dictionary<int, GetServerDataJSExampleV2Data> _dataCallbackUploadingData = new Dictionary<int, GetServerDataJSExampleV2Data>();
    
    private int _identifierRequest = 0;

    [DllImport("__Internal")]
    private static extern void YandexGetServerDataJS(Action<string, int, string, string> callBack, string keyInstanceClass, int indific, string addDataJs);

    
    private bool _init = false;
    public override bool IsInit => _init;
    public override event Action OnInit;

    private void Awake()
    {
        _instancesClasses.Add(_keyInstanceClass, this);

        Init();
    }
    
    private void Init()
    {
        _init = true;
        OnInit?.Invoke();
    }

    public override void GetServerDataJS(Action<int, StatusCallBackServer, SD_DataGetRequestServerJSWrapperAddDataJS, string> callback, int id, string addDataJs, string keyInstanceClass)
    {
        _identifierRequest++;

        var data = new GetServerDataJSExampleV2Data(callback, keyInstanceClass, id);
        _dataCallbackUploadingData.Add(_identifierRequest, data);
        YandexGetServerDataJS(CallbackDataBuyProduct, _keyInstanceClass, _identifierRequest, addDataJs);
    }
    
    [MonoPInvokeCallback(typeof(Action<string, int, string, string>))]
    private static void CallbackDataBuyProduct(string keyInstanceClass, int identifierRequest, string jsonData, string statusData)
    {
        var exampleClass = _instancesClasses[keyInstanceClass];
        var callbackData = exampleClass._dataCallbackUploadingData[identifierRequest];
        exampleClass._dataCallbackUploadingData.Remove(identifierRequest);

        if (jsonData == "null")
        {
            jsonData = "";
        }
        
        SD_DataGetRequestServerJSWrapperAddDataJS dataServer = JsonUtility.FromJson<SD_DataGetRequestServerJSWrapperAddDataJS>(jsonData); 
        StatusCallBackServer statusCallBack = JsonUtility.FromJson<StatusCallBackServer>(statusData);

        callbackData.CallBack.Invoke(callbackData.Id, statusCallBack, dataServer, callbackData.KeyInstanceClass);
    }
    
    private void OnDestroy()
    {
        _instancesClasses.Remove(_keyInstanceClass);
    }
}

public class GetServerDataJSExampleV2Data
{
    public GetServerDataJSExampleV2Data(Action<int, StatusCallBackServer, SD_DataGetRequestServerJSWrapperAddDataJS, string> callback, string keyInstanceClass, int id)
    {
        _callBack = callback;
        _id = id;
        _keyInstanceClass = keyInstanceClass;
    }

    private Action<int, StatusCallBackServer, SD_DataGetRequestServerJSWrapperAddDataJS, string> _callBack;
    private string _keyInstanceClass;
    private int _id;

    public Action<int, StatusCallBackServer, SD_DataGetRequestServerJSWrapperAddDataJS, string> CallBack => _callBack;
    public string KeyInstanceClass => _keyInstanceClass;
    public int Id => _id;
}
#endif