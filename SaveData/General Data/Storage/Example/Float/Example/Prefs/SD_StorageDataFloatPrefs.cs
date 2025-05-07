using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Save Data/Storage Data/Storage/Float/Prefs")]
public class SD_StorageDataFloatPrefs : SD_AbsFloatStorage, IInitScripObj
{
    public override event Action OnInit;
    public override event Action OnUpdateData;
    public override event Action<SD_KeyStorageFloatVariable> OnUpdateValue;
    public override event Action OnLastStatusUpdateData;
    
    public override StatusStorageAction LastStatusUpdateData => StatusStorageAction.Ok;
    //public override StatusStorageAction LastStatusUpdateData => _lastStatusUpdateData;
    //private StatusStorageAction _lastStatusUpdateData = StatusStorageAction.None;
    
    public override event Action OnSaveDataComplited;
    
    public override StatusStorageAction LastStatusSaveData => StatusStorageAction.Ok;
    //public override StatusStorageAction LastStatusSaveData => _lastStatusSaveData;
    //private StatusStorageAction _lastStatusSaveData = StatusStorageAction.None;
    
    public override bool IsInit => _initData;
    private bool _initData = false;
    
    [SerializeField]
    private string targetPathData = "YXY YXY KEY YXY X";
    
    /// <summary>
    /// сохраняймые данные в Js 
    /// </summary>
    [SerializeField] 
    private SD_SaveDataFloadDefListData _saveDataFloadDefListData = new SD_SaveDataFloadDefListData();
    
    /// <summary>
    /// Дефолтные значения переменных
    /// </summary>
    [SerializeField] 
    private List<SD_GetClassKeyDataGetDKOFloat> _defaultValueVariable = new List<SD_GetClassKeyDataGetDKOFloat>();
    private Dictionary<string, float> _keyDefaultValueVariable = new Dictionary<string, float>();

    
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

    [SerializeField] 
    private bool _debugLog = true;
    public void InitScripObj()
    {
#if UNITY_EDITOR
        
        EditorApplication.playModeStateChanged -= OnUpdateStatusPlayMode;
        EditorApplication.playModeStateChanged += OnUpdateStatusPlayMode;

        //На случай если event playModeStateChanged не отработает при входе в режим PlayModeStateChange.EnteredPlayMode (такое может быть, и как минимум по этому нужна пер. bool _init)
        if (EditorApplication.isPlaying == true)
        {
            _initData = false;
            InitializationStorage();
        }
#endif
    }
    
#if UNITY_EDITOR
    private void OnUpdateStatusPlayMode(PlayModeStateChange obj)
    {
    
        //При выходе из Play Mode произвожу очистку данных(тем самым эмулирую что при след. запуске(вхождение в Play Mode) данные будут обнулены)
        if (obj == PlayModeStateChange.ExitingPlayMode)
        {
            if (_initData == true)
            {
                _initData = false;
            }
        }
        
        // При запуске игры эмулирую иниц. SO(По идеи не совсем верно, т.к Awake должен произойти немного раньше, но пофиг)(как показала практика метод может не сработать)
        if (obj == PlayModeStateChange.EnteredPlayMode)
        {
            if (_initData == false)
            {
                InitializationStorage();
            }
            
        }
    }
#endif

    private void Awake()
    {
#if UNITY_EDITOR

#else
  InitializationStorage();
#endif
    }

    public override void InitializationStorage()
    {
        if (_initData == false)
        {
            _keyDefaultValueVariable.Clear();
            AddDefaultValue();
              
            InitStorage();   
        }
    }
      
    private void AddDefaultValue()
    {
        foreach (var VARIABLE in _defaultValueVariable)
        {
            _keyDefaultValueVariable.Add(VARIABLE.GetKey().GetKey(), VARIABLE.DefaulValue);
        }
    }
    
    private void InitStorage()
    {
        _listTaskSave = new ListTask();
        _listTaskUploading = new ListTask();
          
        _storageTaskBlockSave = new SD_StorageTask();
        _storageTaskBlockUploading = new SD_StorageTask();
        
        var dataJS = PlayerPrefs.GetString(targetPathData);
        SD_SaveDataFloadDefListData data = JsonUtility.FromJson<SD_SaveDataFloadDefListData>(dataJS);
          
        if (data == null)
        {
            Debug.LogWarning("Внимание, пришел null с данными в Storage");           
            
            _saveDataFloadDefListData = new SD_SaveDataFloadDefListData();
        }
        else
        {
            _saveDataFloadDefListData = data;
        }

        //_lastStatusUpdateData = StatusStorageAction.Ok;
            
        _initData = true;
        OnInit?.Invoke();
            
        OnUpdateData?.Invoke();
            
        OnLastStatusUpdateData?.Invoke();
    }

    public override void UploadingData(TaskInfo taskInfo, bool urgentUploading = false)
    {
        _listTaskUploading.AddTask(taskInfo);
              
        if (urgentUploading == false)
        {
            if (_storageTaskBlockUploading.IsThereTasks() == false)
            {
                CheckTaskUploading();
            }
            else
            {
                _storageTaskBlockUploading.OnUpdateStatus -= OnUpdateStatusBlockUploading;
                _storageTaskBlockUploading.OnUpdateStatus += OnUpdateStatusBlockUploading;
            }
        }
        else
        {
            CheckTaskUploading();
        }
    }
     
    private void OnUpdateStatusBlockUploading()
    {
        if (_storageTaskBlockUploading.IsThereTasks() == false) 
        {
            _storageTaskBlockUploading.OnUpdateStatus -= OnUpdateStatusBlockUploading;
            CheckTaskUploading();
        }
    }
      
    private void CheckTaskUploading()
    {
        if (_listTaskUploading.IsThereTasks() == true)
        {
            _listTaskUploading.RemoveAllTask();
                  
            StartUpdateData();
        }
    }
    
    private void StartUpdateData()
    {
        DebugLog("Началась выгрузка данных");
        
        var dataJS = PlayerPrefs.GetString(targetPathData);
        SD_SaveDataFloadDefListData data = JsonUtility.FromJson<SD_SaveDataFloadDefListData>(dataJS);
          
        if (data == null)
        {
            Debug.LogWarning("Внимание, пришел null с данными в Storage");    
            _saveDataFloadDefListData = new SD_SaveDataFloadDefListData();
        }
        else
        {
            _saveDataFloadDefListData = data;
        }
        
        
        //_lastStatusUpdateData = StatusStorageAction.Ok;
        
        OnUpdateData?.Invoke();
        
        OnLastStatusUpdateData?.Invoke();
    }
    
    public override void SaveData(TaskInfo taskInfo, bool urgentSaving = false)
    {
        _listTaskSave.AddTask(taskInfo);
          
        if (urgentSaving == false)
        {
            if (_storageTaskBlockSave.IsThereTasks() == false)
            {
                CheckTaskSave();
            }
            else
            {
                _storageTaskBlockSave.OnUpdateStatus -= OnUpdateStatusBlockSave;
                _storageTaskBlockSave.OnUpdateStatus += OnUpdateStatusBlockSave;
            }
        }
        else
        {

            CheckTaskSave();

        }
    }
  
      
    private void OnUpdateStatusBlockSave()
    {   
        if (_storageTaskBlockSave.IsThereTasks() == false)
        {
            _storageTaskBlockSave.OnUpdateStatus -= OnUpdateStatusBlockSave;
              
            CheckTaskSave();
        }
    }
      
    private void CheckTaskSave()
    {
        if (_listTaskSave.IsThereTasks() == true)
        {
            _listTaskSave.RemoveAllTask();
            StartSaveData();
        }
    }    

    
    private  void StartSaveData()
    {
        DebugLog("Началось сохранение данных");
        
        string data = JsonUtility.ToJson(_saveDataFloadDefListData);
        PlayerPrefs.SetString(targetPathData, data);
        PlayerPrefs.Save();
        
        //_lastStatusSaveData = StatusStorageAction.Ok;
        OnSaveDataComplited?.Invoke();
    }
    
    public override bool IsThereData(SD_KeyStorageFloatVariable dataKey)
    {
        return _saveDataFloadDefListData.IsThereData(dataKey.GetKey());
    }

    public override float GetData(SD_KeyStorageFloatVariable dataKeyVariable)
    {
        if (IsThereData(dataKeyVariable) == false)
        {
            if (_keyDefaultValueVariable.ContainsKey(dataKeyVariable.GetKey()) == true)
            {
                return _keyDefaultValueVariable[dataKeyVariable.GetKey()];
            }
            
            return default;
        }
        
        return _saveDataFloadDefListData.GetValue(dataKeyVariable.GetKey());
    }

    public override void SetData(SD_KeyStorageFloatVariable dataKey, float data)
    {
        _saveDataFloadDefListData.SetValue(dataKey.GetKey(), data);
        OnUpdateValue?.Invoke(dataKey);
    }

    public override bool IsWaitingResponseSaveData()
    {
        return false;
    }
    
    public override bool IsWaitingResponseUploadingData()
    {
        return false;
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

    public override IReadOnlyList<SD_KeyStorageFloatVariable> GetListKey()
    {
        List<SD_KeyStorageFloatVariable> listKey = new List<SD_KeyStorageFloatVariable>();
       
        foreach (var VARIABLE in _saveDataFloadDefListData.GetListKey())
        {
            listKey.Add(new SD_KeyStorageFloatVariable(VARIABLE));
        }
        
        return listKey;
    }
    
      
    public override SD_StorageTask GetStorageTaskBlockSave()
    {
        return _storageTaskBlockSave;
    }  
      
    public override SD_StorageTask GetStorageTaskBlockUploading()
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

    
    
    
    
    
    
    
    
