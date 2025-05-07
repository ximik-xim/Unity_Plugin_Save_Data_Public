using System;
using System.Collections;
using System.Collections.Generic;
using AOT;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Save Data/Storage Data/Storage/Float/Server")]
public class SD_StorageFloatDataServer : SD_AbsFloatStorage, IInitScripObj
{
    public override event Action OnInit;
    public override event Action OnUpdateData;
    public override event Action<SD_KeyStorageFloatVariable> OnUpdateValue;
    public override event Action OnLastStatusUpdateData;
    public override StatusStorageAction LastStatusUpdateData => _lastStatusUpdateData;
    private StatusStorageAction _lastStatusUpdateData = StatusStorageAction.None;
    
    public override event Action OnSaveDataComplited;
    public override StatusStorageAction LastStatusSaveData => _lastStatusSaveData;
    private StatusStorageAction _lastStatusSaveData = StatusStorageAction.None;

    public override bool IsInit => _initData;
    private bool _initData = false;
    
    /// <summary>
    /// сохраняймые данные в Js 
    /// </summary>
    [SerializeField] 
    private SD_SaveDataFloadDefListData _saveDataFloatDefListData = new SD_SaveDataFloadDefListData();
    
    /// <summary>
    /// Дефолтные значения переменных
    /// </summary>
    [SerializeField] 
    private List<SD_GetClassKeyDataGetDKOFloat> _defaultValueVariable = new List<SD_GetClassKeyDataGetDKOFloat>();
    private Dictionary<string, float> _keyDefaultValueVariable = new Dictionary<string, float>();
    
    /// <summary>
    /// Логика серверного хранилеща
    /// </summary>
    [SerializeField] 
    private SD_ServerStorageDataLogicJS _serverStorageDataLogic;

    /// <summary>
    /// Ключ, по которому буду получать данные с сервера
    /// </summary>
    [SerializeField] 
    private GetDataSO_SD_KeyStorageServerJS _getDataKeyServerStorageDataLogic;
    
    /// <summary>
    /// Тип подписки на обновление данные
    /// в1) EventLast - будет срабатывать при каждом успешной отправке(или получении) данных
    /// в2) EventLast - будет срабатывать, когда все Task на отправку(или получения) данных будут УСПЕШНО(статус ОК) отправлены(или получены)
    /// </summary>
    [SerializeField] 
    private ServerStorageTypeSubscriptionEvent _typeUploadingData;
    
    [SerializeField] 
    private ServerStorageTypeSubscriptionEvent _typeSaveData;
    
     public void InitScripObj()
    {
#if UNITY_EDITOR
        _initData = false;
#endif
    }
    
    public override void InitializationStorage()
    {
        if (_initData == false)
        {
            _keyDefaultValueVariable.Clear();
            AddDefaultValue();
            
            if (_serverStorageDataLogic.IsInit == true)
            {
                InitStorageServer();
            }
            else
            {
                _serverStorageDataLogic.OnInit -= OnInitStorageServer;
                _serverStorageDataLogic.OnInit += OnInitStorageServer;
            }
            
        }
    }
    
    private void AddDefaultValue()
    {
        foreach (var VARIABLE in _defaultValueVariable)
        {
            _keyDefaultValueVariable.Add(VARIABLE.GetKey().GetKey(), VARIABLE.DefaulValue);
        }
    }
    
    private void OnInitStorageServer()
    {
        _serverStorageDataLogic.OnInit -= OnInitStorageServer;

        InitStorageServer();
    }

    private void InitStorageServer()
    {
        _serverStorageDataLogic.Add(_getDataKeyServerStorageDataLogic.GetData(), new SD_GetJsStorage(GetDataJS));

        InitCheckLastStatusStorageServer();
    }
    
    private string GetDataJS()
    {
        return JsonUtility.ToJson(_saveDataFloatDefListData);
    }
    
    private void InitCheckLastStatusStorageServer()
    {
        if (_serverStorageDataLogic.LastStatusUpdateData == StatusStorageAction.Ok)
        {
            InitUpdateDataStorage();
        }
        else
        {
            _serverStorageDataLogic.OnLastStatusUpdateData -= OnInitUpdateLastStatusStorageServer;
            _serverStorageDataLogic.OnLastStatusUpdateData += OnInitUpdateLastStatusStorageServer;
        }
    }


    private void OnInitUpdateLastStatusStorageServer()
    {
        if (_serverStorageDataLogic.LastStatusUpdateData == StatusStorageAction.Ok)
        {
            _serverStorageDataLogic.OnLastStatusUpdateData -= OnInitUpdateLastStatusStorageServer;

            InitUpdateDataStorage();
        }
        else
        {
            Debug.Log("Внимание, похоже от сервера пришел ERROR, при инициализации JS хранилеща Bool");
        }
    }

    private void InitUpdateDataStorage()
    {
        UploadingData();

        if (_typeUploadingData == ServerStorageTypeSubscriptionEvent.EventLast)
        {
            _serverStorageDataLogic.OnLastStatusUpdateData -= CallbackUploadingData;
            _serverStorageDataLogic.OnLastStatusUpdateData += CallbackUploadingData;
        }
        else
        {
            _serverStorageDataLogic.OnComlpitedUpdateData -= CallbackUploadingData;
            _serverStorageDataLogic.OnComlpitedUpdateData += CallbackUploadingData;
        }


        if (_typeSaveData == ServerStorageTypeSubscriptionEvent.EventLast)
        {
            _serverStorageDataLogic.OnLastStatusSave -= CallbackLastStatusSaveData;
            _serverStorageDataLogic.OnLastStatusSave += CallbackLastStatusSaveData;
        }
        else
        {
            _serverStorageDataLogic.OnComlpitedSave -= CallbackLastStatusSaveData;
            _serverStorageDataLogic.OnComlpitedSave += CallbackLastStatusSaveData;
        }
        
        _initData = true;
        OnInit?.Invoke();
    }
    
    private void UploadingData()
    {
        _lastStatusUpdateData = _serverStorageDataLogic.LastStatusUpdateData;
        
        if (_serverStorageDataLogic.LastStatusUpdateData == StatusStorageAction.Ok)
        {
            var dataJS = _serverStorageDataLogic.GetJSData(_getDataKeyServerStorageDataLogic.GetData());
            SD_SaveDataFloadDefListData data = JsonUtility.FromJson<SD_SaveDataFloadDefListData>(dataJS);
          
            if (data == null)
            {
                Debug.LogError("Внимание, пришел null с данными в Storage");    
                _saveDataFloatDefListData = new SD_SaveDataFloadDefListData();
            }
            else
            {
                _saveDataFloatDefListData = data;
            }
            
            OnUpdateData?.Invoke();
        }
        
        OnLastStatusUpdateData?.Invoke();
    }
    
    private void CallbackUploadingData()
    {
        UploadingData();
    }
    
    private void CallbackLastStatusSaveData()
    {
        _lastStatusSaveData = _serverStorageDataLogic.LastStatusSaveData;
        OnSaveDataComplited?.Invoke();
    }
    
    public override void SaveData(TaskInfo taskInfo, bool urgentSaving = false)
    {
        _serverStorageDataLogic.SaveData(taskInfo, urgentSaving);
    }

    public override void UploadingData(TaskInfo taskInfo, bool urgentUploading = false)
    {
        _serverStorageDataLogic.UploadingData(taskInfo, urgentUploading);
    }

    public override bool IsThereData(SD_KeyStorageFloatVariable dataKey)
    {
        return _saveDataFloatDefListData.IsThereData(dataKey.GetKey());
    }

    public override float GetData(SD_KeyStorageFloatVariable dataKey)
    {
        if (IsThereData(dataKey) == false)
        {
            if (_keyDefaultValueVariable.ContainsKey(dataKey.GetKey()) == true)
            {
                return _keyDefaultValueVariable[dataKey.GetKey()];
            }
            
            return default;
        }
        
        return _saveDataFloatDefListData.GetValue(dataKey.GetKey());
    }

    public override void SetData(SD_KeyStorageFloatVariable dataKey, float data)
    {
        _saveDataFloatDefListData.SetValue(dataKey.GetKey(), data);
        OnUpdateValue?.Invoke(dataKey);
    }
    
    public override IReadOnlyList<SD_KeyStorageFloatVariable> GetListKey()
    {
        List<SD_KeyStorageFloatVariable> listKey = new List<SD_KeyStorageFloatVariable>();
       
        foreach (var VARIABLE in _saveDataFloatDefListData.GetListKey())
        {
            listKey.Add(new SD_KeyStorageFloatVariable(VARIABLE));
        }
        
        return listKey;
    }
    
    public override void AddDefValue(SD_KeyStorageFloatVariable key, float data)
    {
        _keyDefaultValueVariable.Add(key.GetKey(),data);
    }

    public override float GetDefValue(SD_KeyStorageFloatVariable key)
    {
        return _keyDefaultValueVariable[key.GetKey()];
    }

    public override bool IsThereDataDef(SD_KeyStorageFloatVariable dataKey)
    {
        return _keyDefaultValueVariable.ContainsKey(dataKey.GetKey());
    }

    public override bool IsWaitingResponseSaveData()
    {
        return _serverStorageDataLogic.isStartSaveData;
    }
    
    public override bool IsWaitingResponseUploadingData()
    {
        return _serverStorageDataLogic.IsStartUploadingData;
    }
    
    
    public override SD_StorageTask GetStorageTaskBlockSave()
    {
        return _serverStorageDataLogic.GetStorageTaskBlockSave();
    }  
      
    public override SD_StorageTask GetStorageTaskBlockUploading()
    {
        return _serverStorageDataLogic.GetStorageTaskBlockUploading();
    } 
}
