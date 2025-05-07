 using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
 
[CreateAssetMenu(menuName = "Save Data/Server/Server JS")]
     /// <summary>
     /// В этой реализации логики общения с сервером, следующие особенности 
     /// Каждый следующих  запрос на сохранение, будет стирать предыдущие подготовленные данные для отправке на сервер
     /// Тем самым гарантируется, что на сервер будут отправляться, только последний(они же актуальные) данные
     /// (если будет несколько запросов на сохранение подряд) 
     /// </summary>
public class SD_ServerStorageDataLogicJS : ScriptableObject, IInitScripObj
{
    [SerializeField] 
    private string _keyInstanceClass = "";

    //Хранит в себе информацию для работы с хранилищами( для взятия из них данных)
    private Dictionary<string, SD_GetJsStorage> _storageData = new Dictionary<string, SD_GetJsStorage>();

    //Нужен что бы нормально сериализовать словарь(а так же что бы в инспекторе было видно данные)
    [SerializeField] 
    private ServerStorageDataLogicBuffer _bufferData = new ServerStorageDataLogicBuffer();
    
    ///Нужен в случае если идет сохранение, что бы сохранить текущие данные 
    [SerializeField] 
    private ServerStorageDataLogicBuffer _saveBufferWating = new ServerStorageDataLogicBuffer();
    //Тут уже буду данные в виде JS(которые будем брать по ключу)
    private Dictionary<string, string> _jsData = new Dictionary<string, string>();

    private DontDestroyMBS_DKO destroyMbsDko;
    [SerializeField] 
    private GetDataSO_MBS_DKO _keyDkoGetDataInPlayer;
    private DKOKeyAndTargetAction _DkoGetDataInPlayer;
    [SerializeField] 
    private GetDataSODataDKODataKey _keyGeneralLogicGetDataInPlayer;

    [SerializeField] 
    private GetDataSO_MBS_DKO _keyDkoSetDataInPlayer;
    private DKOKeyAndTargetAction _DkoSetDataInPlayer;
    [SerializeField] 
    private GetDataSODataDKODataKey _keyGeneralLogicSetDataInPlayer;
    
    private SD_GetServerDataJSWrapper _getDataInPlayer;
    private SD_SetServerDataJSWrapper _setDataInPlayer;

    public event Action OnInit;
    public bool IsInit => _initData;
    private bool _initData = false;
    
    private bool _isStartUploadingData = false;
    private bool _isStartSaveData = false;

    public bool IsStartUploadingData => _isStartUploadingData;
    public bool isStartSaveData => _isStartSaveData;

    /// <summary>
    /// Сработает когда закончиться сохранение(получу OK от сервера)
    /// </summary>
    public event Action OnComlpitedSave;
    
    /// <summary>
    /// Сработает когда закончиться выгрузка данных(получу OK от сервера)
    /// </summary>
    public event Action OnComlpitedUpdateData;
    
    /// <summary>
    /// Пришел статус попытки отправить сохранение
    /// (учесть, что в случае ошибки, при отправке, будет автоматически переотправка)
    /// </summary>
    public  event Action OnLastStatusSave;
    
    /// <summary>
    /// Пришел статус попытки получить данные с сервера
    /// (учесть, что в случае ошибки, при отправке, будет автоматически переотправка)
    /// </summary>
    public  event Action OnLastStatusUpdateData;
    public  StatusStorageAction LastStatusUpdateData => _lastStatusUpdateData;
    private StatusStorageAction _lastStatusUpdateData = StatusStorageAction.None;
    
    public  StatusStorageAction LastStatusSaveData => _lastStatusSaveData;
    private StatusStorageAction _lastStatusSaveData = StatusStorageAction.None;
    
    /// <summary>
    /// Списки задач на сохранение и выгрузку
    /// </summary>
    private ListTask _listTaskSave = new ListTask();
    private ListTask _listTaskUploading = new ListTask();

    /// <summary>
    /// Списки задач на блокировку сохранение и выгрузку
    /// </summary>
    private SD_StorageTask _storageTaskBlockSave;
    private SD_StorageTask _storageTaskBlockUploading;

    /// <summary>
    /// Ключи блокировки для сохранения и выгрузки
    /// Нужны когда достигним максимума при кол-ве попыток переполучить или переотправить данные на сервер
    /// </summary>
    private SD_KeyStorageTask _keyBlockSave;
    private SD_KeyStorageTask _keyBlockUploading;
    
    /// <summary>
    /// Логика ошибки на выгрузку и сохранение
    /// (по сути ведет счет ошибок, и если достигли N знач., то запрещает переотправку запроса, пока что то не произойдет)
    /// </summary>
    [SerializeField] 
    private SD_ErrorLogicForwardingData _errorUploading;
    [SerializeField] 
    private SD_ErrorLogicForwardingData _errorSave;
    
    private  GetServerRequestData<SD_DataGetRequestServerJSWrapperAddDataJS> _dataCallbackGetDataServerInit;
    private  GetServerRequestData<SD_DataGetRequestServerJSWrapperAddDataJS> _dataCallbackGetDataServer;
    
    private  GetServerRequestData<SD_DataSetRequestServerJSWrapperAddDataJS> _dataCallbackSetDataServer;
    
    [SerializeField] 
    private bool _debugLog = true;
    public void InitScripObj()
    {
#if UNITY_EDITOR
        _initData = false;

        _isStartUploadingData = false;
        _isStartSaveData = false;
#endif
    }
    
    
    public void InitializationStorage()
    {
        if (_initData == false)
        {
            _storageData.Clear();
            _bufferData._buffer.Clear();
            _jsData.Clear();

            destroyMbsDko = GameObject.FindObjectOfType<DontDestroyMBS_DKO>();

            if (destroyMbsDko == null)
            {
                SceneManager.sceneLoaded -= OnLoadScene;
                SceneManager.sceneLoaded += OnLoadScene;
                return;
            }
            
            FindMbsDkoDestroy();
        }
    }
    
    private void OnLoadScene(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= OnLoadScene;
        FindMbsDkoDestroy();
    }

    private void FindMbsDkoDestroy()
    {
        destroyMbsDko = GameObject.FindObjectOfType<DontDestroyMBS_DKO>();

        if (destroyMbsDko.IsInit == false)
        {
            destroyMbsDko.OnInit -= OnInitMBS_DKO;
            destroyMbsDko.OnInit += OnInitMBS_DKO;
            
            return;
        }

        GetDKOGeneralLogic();
    }

    private void OnInitMBS_DKO()
    {
        destroyMbsDko.OnInit -= OnInitMBS_DKO;
        GetDKOGeneralLogic();
    }

    private void GetDKOGeneralLogic()
    { 
        var DKOGetDataInPlayer = destroyMbsDko.GetDKO(_keyDkoGetDataInPlayer.GetData());
        var generalLogicGetDataInPlayer= DKOGetDataInPlayer.KeyRun<DKODataInfoT<SD_GetServerDataJSWrapper>>(_keyGeneralLogicGetDataInPlayer.GetData());
        _getDataInPlayer = generalLogicGetDataInPlayer.Data;


        var DKOSetDataInPlayer = destroyMbsDko.GetDKO(_keyDkoSetDataInPlayer.GetData());
        var generalLogicSetDataInPlayer= DKOSetDataInPlayer.KeyRun<DKODataInfoT<SD_SetServerDataJSWrapper>>(_keyGeneralLogicSetDataInPlayer.GetData());
        _setDataInPlayer = generalLogicSetDataInPlayer.Data;
            
        InitTaskStorage();   
    }
    
    private void InitTaskStorage()
    {
        _listTaskSave = new ListTask();
        _listTaskUploading = new ListTask();
          
        _storageTaskBlockSave = new SD_StorageTask();
        _storageTaskBlockUploading = new SD_StorageTask();

        _keyBlockSave = new SD_KeyStorageTask("Ключ блокировки, привешение запросов на сохранение");
        _keyBlockUploading = new SD_KeyStorageTask("Ключ блокировки, привешение запросов на выгрузку");
        
        _errorSave.OnUpdateData -= OnUpdateDataErrorSave;
        _errorSave.OnUpdateData += OnUpdateDataErrorSave;
        
        _errorUploading.OnUpdateData -= OnUpdateDataErrorUploading;
        _errorUploading.OnUpdateData += OnUpdateDataErrorUploading;

        OnUpdateDataErrorSave();
        OnUpdateDataErrorUploading();
        
        GetDataServerInit();
    }
 
    /// <summary>
    /// Нужен, что бы когда счетчик кол-во error запросов заблокирует переотправку, сразу добавили Task на блокировку отпр. сохр
    /// </summary>
    private void OnUpdateDataErrorSave()
    {
        if (_errorSave.IsContinue == true)
        {
            if (_storageTaskBlockSave.IsKeyTask(_keyBlockSave) == true) 
            {
                _storageTaskBlockSave.RemoveTask(_keyBlockSave);
            }
        }
        else
        {
            if (_storageTaskBlockSave.IsKeyTask(_keyBlockSave) == false) 
            {
                _storageTaskBlockSave.AddTask(_keyBlockSave,"Достиг лимита переотправки запроса на сохранения");
            }
        }
    }
    
    /// <summary>
    /// Нужен, что бы когда счетчик кол-во error запросов заблокирует переотправку, сразу добавили Task на блокировку отпр. сохр
    /// </summary>
    private void OnUpdateDataErrorUploading()
    {
        if (_errorUploading.IsContinue == true)
        {
            if (_storageTaskBlockUploading.IsKeyTask(_keyBlockUploading) == true) 
            {
                _storageTaskBlockUploading.RemoveTask(_keyBlockUploading);
            }
        }
        else
        {
            if (_storageTaskBlockUploading.IsKeyTask(_keyBlockUploading) == false)
            {
                _storageTaskBlockUploading.AddTask(_keyBlockUploading, "Достиг лимита переотправки запроса на выгрузку данных");
            }
        }
    }
    
    private void GetDataServerInit()
    {
        _dataCallbackGetDataServerInit = _getDataInPlayer.GetServerDataJS();

        if (_dataCallbackGetDataServerInit.IsGetDataCompleted == false)
        {
            _dataCallbackGetDataServerInit.OnGetDataCompleted -= OnCallbackGetDataServerInit;
            _dataCallbackGetDataServerInit.OnGetDataCompleted += OnCallbackGetDataServerInit;
            return;
        }

        CallbackGetDataServerInit();
    }
    
    private void OnCallbackGetDataServerInit()
    {
        if (_dataCallbackGetDataServerInit.IsGetDataCompleted == true)
        {
            _dataCallbackGetDataServerInit.OnGetDataCompleted -= OnCallbackGetDataServerInit;
            CallbackGetDataServerInit();    
        }
    }
    
    private void CallbackGetDataServerInit()
    {
        if (_dataCallbackGetDataServerInit.StatusServer == StatusCallBackServer.Ok)
        {
            string dataJS = _dataCallbackGetDataServerInit.GetData.ServerDataJS.DataJS;
            _bufferData = JsonUtility.FromJson<ServerStorageDataLogicBuffer>(dataJS);
                
            if (_bufferData == null)
            {
                Debug.LogError("Внимание, пришел null с данными в логике Storage");
                _bufferData = new ServerStorageDataLogicBuffer();
            }
                
            foreach (var VARIABLE in _bufferData._buffer)
            {
                if (_jsData.ContainsKey(VARIABLE.Key) == false)
                {
                    _jsData.Add(VARIABLE.Key, VARIABLE.Data);
                    continue;
                }

                _jsData[VARIABLE.Key] = VARIABLE.Data;
            }
                
            _lastStatusUpdateData = StatusStorageAction.Ok;
            OnLastStatusUpdateData?.Invoke();
                
            _initData = true;
            OnInit?.Invoke();
        }
        else
        {
            Debug.LogError("Ошибка при иниц. сервера");
            
            //если не удалось получить данные с сервера, делаю еще раз запроса(до бесконечности)
            //Возможно потом придумаю что то получше...
            GetDataServerInit();
        }
    }

    public void UploadingData(TaskInfo taskInfo, bool urgentSaving = false)
    {
        _listTaskUploading.AddTask(taskInfo);
        
        CheckTaskUploadingData(urgentSaving);
    }
    
    private void CheckTaskUploadingData(bool urgentUploading)
    {
        if (_isStartUploadingData == false)
        {
            if (urgentUploading == false)
            {
                if (_storageTaskBlockUploading.IsThereTasks() == false)
                {
                    _listTaskUploading.RemoveAllTask();
        
                    StartUpdateData();
                }
                else
                {
                    _storageTaskBlockUploading.OnUpdateStatus -= OnCheckDeletedTaskBlockUploading;
                    _storageTaskBlockUploading.OnUpdateStatus += OnCheckDeletedTaskBlockUploading;
                }

            }
            else
            {
                _listTaskUploading.RemoveAllTask();
        
                StartUpdateData();
            }
        }
        else
        {
            //дальше по логике, буду проверять налияие Task на выгрузку, по этому тут их не трогаю
        }
    }
    

    private void OnCheckDeletedTaskBlockUploading()
    {
        if (_storageTaskBlockUploading.IsThereTasks() == false)
        {
            _storageTaskBlockUploading.OnUpdateStatus -= OnCheckDeletedTaskBlockUploading;
            
            if (_listTaskUploading.IsThereTasks() == true)
            {
                _listTaskUploading.RemoveAllTask();
        
                StartUpdateData();    
            }
            
        }
           
    }
    
    private void StartUpdateData()
    { 
        if (_isStartUploadingData == false)
        {
            DebugLog("Началась выгрузка данных");
            
            //На случай если запрос пришел в обход блокировки на отправку(срочный запрос)
            _storageTaskBlockUploading.OnUpdateStatus -= OnCheckDeletedTaskBlockUploading;
            _storageTaskBlockUploading.OnUpdateStatus -= OnCheckDeletedTaskBlockUploadingResetPush;
            
            //навсякий случай доп проверка
            if (_dataCallbackGetDataServer != null) 
            {
                _dataCallbackGetDataServer.OnGetDataCompleted -= OnCallbackGetDataServer;
                _dataCallbackGetDataServer.OnGetDataCompleted -= OnCallbackGetDataServerReset;
            }
            
            _isStartUploadingData = true;
            _dataCallbackGetDataServer = _getDataInPlayer.GetServerDataJS();
            
            if (_dataCallbackGetDataServer.IsGetDataCompleted == false)
            {
                _dataCallbackGetDataServer.OnGetDataCompleted -= OnCallbackGetDataServer;
                _dataCallbackGetDataServer.OnGetDataCompleted += OnCallbackGetDataServer;
                return;
            }

            CallbackGetDataServer();
        }
     
    }
    
    private void OnCallbackGetDataServer()
    {
        if (_dataCallbackGetDataServer.IsGetDataCompleted == true)
        {
            _dataCallbackGetDataServer.OnGetDataCompleted -= OnCallbackGetDataServer;
            CallbackGetDataServer();    
        }
    }

    private void CallbackGetDataServer()
    {
        DebugLog("Был пулчен ответ от выгрузки данных данных = " + _dataCallbackGetDataServer.StatusServer);
        if (_dataCallbackGetDataServer.StatusServer == StatusCallBackServer.Ok)
        {
            _errorUploading.OnRemoveAllError();

            string dataJS = _dataCallbackGetDataServer.GetData.ServerDataJS.DataJS;
            _bufferData = JsonUtility.FromJson<ServerStorageDataLogicBuffer>(dataJS);

            if (_bufferData == null)
            {
                Debug.LogError("Внимание, пришел null с данными в логике Storage");
                _bufferData = new ServerStorageDataLogicBuffer();
            }

            foreach (var VARIABLE in _bufferData._buffer)
            {
                if (_jsData.ContainsKey(VARIABLE.Key) == false)
                {
                    _jsData.Add(VARIABLE.Key, VARIABLE.Data);
                    continue;
                }

                _jsData[VARIABLE.Key] = VARIABLE.Data;
            }
            
            _lastStatusUpdateData = StatusStorageAction.Ok;
            OnLastStatusUpdateData?.Invoke();
            
            //если есть Task на выгрузку, то начинаю след. выгрузку
            if (_listTaskUploading.IsThereTasks() == true)
            {
                RestartPushUploading();
                return;
            }

            _isStartUploadingData = false;
            OnComlpitedUpdateData?.Invoke();
        }
        else
        {
            _errorUploading.OnAddError();

            if (_storageTaskBlockUploading.IsThereTasks() == true)
            {
                _storageTaskBlockUploading.OnUpdateStatus -= OnCheckDeletedTaskBlockUploadingResetPush;
                _storageTaskBlockUploading.OnUpdateStatus += OnCheckDeletedTaskBlockUploadingResetPush;

                _lastStatusUpdateData = StatusStorageAction.Error;
                OnLastStatusUpdateData?.Invoke();
            }
            else
            {
                RestartPushUploading();
            }
        }
    }
    
    private void OnCheckDeletedTaskBlockUploadingResetPush()
    {
        if (_storageTaskBlockUploading.IsThereTasks() == false)
        {
            _storageTaskBlockUploading.OnUpdateStatus -= OnCheckDeletedTaskBlockUploadingResetPush;

            RestartPushUploading();
        }
    }
    
    private void RestartPushUploading()
    { 
        _listTaskUploading.RemoveAllTask();
        
        //Это навсякий случай доп проверка    
        if (_dataCallbackGetDataServer != null)
        {
            _dataCallbackGetDataServer.OnGetDataCompleted -= OnCallbackGetDataServerReset;
        }

        _dataCallbackGetDataServer = _getDataInPlayer.GetServerDataJS();

        if (_dataCallbackGetDataServer.IsGetDataCompleted == false)
        {
            _dataCallbackGetDataServer.OnGetDataCompleted -= OnCallbackGetDataServerReset;
            _dataCallbackGetDataServer.OnGetDataCompleted += OnCallbackGetDataServerReset;
            return;
        }

        CallbackGetDataServer();
    }
    
    private void OnCallbackGetDataServerReset()
    {
        _dataCallbackGetDataServer.OnGetDataCompleted -= OnCallbackGetDataServerReset;
        CallbackGetDataServer();
    }
    
    
    public void SaveData(TaskInfo taskInfo, bool urgentSaving)
    {
        _listTaskSave.AddTask(taskInfo);
        
        CheckTaskSaveData(urgentSaving);
    }
    
    private void CheckTaskSaveData(bool urgentSaving)
    {
        if (isStartSaveData == false)
        {
            if (urgentSaving == false)
            {
                if (_storageTaskBlockSave.IsThereTasks() == false)
                {
                    _saveBufferWating._buffer.Clear();
                    InsertDataBuffer();
                    
                    _listTaskSave.RemoveAllTask();
                    StartSaveData();
                }
                else
                {
                    _storageTaskBlockSave.OnUpdateStatus -= OnCheckDeletedTaskBlockSave;
                    _storageTaskBlockSave.OnUpdateStatus += OnCheckDeletedTaskBlockSave;
                }
            }
            else
            {
                _saveBufferWating._buffer.Clear();
                InsertDataBuffer();
            
                _listTaskSave.RemoveAllTask();
                StartSaveData();
            }
        }
        else
        {
            _saveBufferWating._buffer.Clear();
            foreach (var VARIABLE in _storageData.Keys)
            {
                string jsData = _storageData[VARIABLE].GetJSData();
                _saveBufferWating._buffer.Add(new AbsKeyData<string, string>(VARIABLE, jsData));
            }
            
            //тут очищаю список Task т.к, дальше ориентируюсь на буффер с данными, а не на список с Task
            _listTaskSave.RemoveAllTask();
        }
    }
    
    private void OnCheckDeletedTaskBlockSave()
    {   
        if (_storageTaskBlockSave.IsThereTasks() == false)
        {
            if (_isStartSaveData == false)
            {
                _storageTaskBlockSave.OnUpdateStatus -= OnCheckDeletedTaskBlockSave;
                
                _saveBufferWating._buffer.Clear();
                InsertDataBuffer();
            
                _listTaskSave.RemoveAllTask();
                StartSaveData();
            }
          
        }
    }
    
    /// <summary>
    /// Заполняю буфер служующий для сериализации словаря
    /// </summary>
    private void InsertDataBuffer()
    {
        _bufferData._buffer.Clear();
            
        foreach (var VARIABLE in _storageData.Keys)
        {
            string jsData = _storageData[VARIABLE].GetJSData();
            _bufferData._buffer.Add(new AbsKeyData<string, string>(VARIABLE, jsData));
        }
    }

    private void StartSaveData()
    {
        if (_isStartSaveData == false)
        {
            DebugLog("Началось сохранение данных");
            
            //На случай если запрос пришел в обход блокировки на отправку(срочный запрос)
            _storageTaskBlockSave.OnUpdateStatus -= OnCheckDeletedTaskBlockSave;
            _storageTaskBlockSave.OnUpdateStatus -= OnCheckDeletedTaskBlockSaveResetPush;
            
            _isStartSaveData = true;
            
              string jsPush = JsonUtility.ToJson(_bufferData);
              
              //Это навсякий случай доп проверка    
              if (_dataCallbackSetDataServer != null)
              {
                  _dataCallbackSetDataServer.OnGetDataCompleted -= OnCallbackSetDataServer;
                  _dataCallbackSetDataServer.OnGetDataCompleted -= OnCallbackSetDataServerPushReset;
              }
                  
              _dataCallbackSetDataServer = _setDataInPlayer.SetServerDataJS(jsPush);

              if (_dataCallbackSetDataServer.IsGetDataCompleted == false)
              {
                  _dataCallbackSetDataServer.OnGetDataCompleted -= OnCallbackSetDataServer;
                  _dataCallbackSetDataServer.OnGetDataCompleted += OnCallbackSetDataServer;
                  return;
              }

              CallbackSetDataServer();
        }
    }

    private void OnCallbackSetDataServer()
    {
        _dataCallbackSetDataServer.OnGetDataCompleted -= OnCallbackSetDataServer;
        CallbackSetDataServer();
    }

    private void CallbackSetDataServer()
    {
        DebugLog("Был пулчен ответ от сохранение данных = " + _dataCallbackSetDataServer.StatusServer);
        if (_dataCallbackSetDataServer.StatusServer == StatusCallBackServer.Ok)
        {
            _errorSave.OnRemoveAllError();

            _lastStatusSaveData = StatusStorageAction.Ok;
            OnLastStatusSave?.Invoke();

            if (_saveBufferWating._buffer.Count > 0)
            {
                PushNextDataSaveBufferData();
                return;
            }

            _isStartSaveData = false;
            OnComlpitedSave?.Invoke();
        }
        else
        {
            _errorSave.OnAddError();

            if (_storageTaskBlockSave.IsThereTasks() == true)
            {
                _storageTaskBlockSave.OnUpdateStatus -= OnCheckDeletedTaskBlockSaveResetPush;
                _storageTaskBlockSave.OnUpdateStatus += OnCheckDeletedTaskBlockSaveResetPush;

                _lastStatusSaveData = StatusStorageAction.Error;
                OnLastStatusSave?.Invoke();
            }
            else 
            {
                PushNextDataSaveBufferData();
            }
        }
    }

    private void OnCheckDeletedTaskBlockSaveResetPush()
    {
        if (_storageTaskBlockSave.IsThereTasks() == false)
        {
            _storageTaskBlockSave.OnUpdateStatus -= OnCheckDeletedTaskBlockSaveResetPush;

            PushNextDataSaveBufferData();
        }
    }

    private void PushNextDataSaveBufferData()
    {
        if (_saveBufferWating._buffer.Count > 0)
        {
            _bufferData._buffer.Clear();

            foreach (var VARIABLE in _saveBufferWating._buffer)
            {
                _bufferData._buffer.Add(VARIABLE);
            }

            _saveBufferWating._buffer.Clear();
        }

        RestartPushSave();
    }

    private void RestartPushSave()
    {
        string jsPush = JsonUtility.ToJson(_bufferData);
        
        _dataCallbackSetDataServer = _setDataInPlayer.SetServerDataJS(jsPush);
        
        //Это навсякий случай доп проверка    
        if (_dataCallbackSetDataServer != null)
        {
            _dataCallbackSetDataServer.OnGetDataCompleted -= OnCallbackSetDataServerPushReset;
        }
                  
        _dataCallbackSetDataServer = _setDataInPlayer.SetServerDataJS(jsPush);

        if (_dataCallbackSetDataServer.IsGetDataCompleted == false)
        {
            _dataCallbackSetDataServer.OnGetDataCompleted -= OnCallbackSetDataServerPushReset;
            _dataCallbackSetDataServer.OnGetDataCompleted += OnCallbackSetDataServerPushReset;
            return;
        }

        CallbackSetDataServer();
    }
    
    private void OnCallbackSetDataServerPushReset()
    {
        _dataCallbackSetDataServer.OnGetDataCompleted -= OnCallbackSetDataServerPushReset;
        CallbackSetDataServer();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   

    

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Add(SD_KeyStorageServerJS key, SD_GetJsStorage data)
    {
        _storageData.Add(key.GetKey(), data);
    }

    public string GetJSData(SD_KeyStorageServerJS key)
    {
        if (_jsData.ContainsKey(key.GetKey()) == true)
        {
            return _jsData[key.GetKey()];    
        }

        return "";
    }
         
    public SD_StorageTask GetStorageTaskBlockSave()
    {
        return _storageTaskBlockSave;
    }  
      
    public SD_StorageTask GetStorageTaskBlockUploading()
    {
        return _storageTaskBlockUploading;
    }  
    
    private void DebugLog(string text)
    {
        if (_debugLog == true) 
        {
            Debug.Log(text);    
        }
    }
}
 
/// <summary>
/// Обертка нужна только что бы нормально сериализовать через JsonUtility
/// </summary>
[System.Serializable]
public class ServerStorageDataLogicBuffer
{
    /// <summary>
    /// Не изменять имя, т.к оно используеться в пути JS
    /// </summary>
    [SerializeField]
    public List<AbsKeyData<string, string>> _buffer = new List<AbsKeyData<string, string>>();
}

