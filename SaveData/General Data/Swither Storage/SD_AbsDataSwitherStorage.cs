using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Переключатель между местами сохранения данных(между хранилещами с данными)
/// Хранит в себе ссылки на хранилеща(опр. типа, пр float) по ключу
/// </summary>
public abstract class SD_AbsDataSwitherStorage<TypeStorage, Key, TypeData, ClassDefaultKey> : MonoBehaviour where TypeStorage : SD_AbsStorage<Key, TypeData> where ClassDefaultKey : SD_AbsDefaultValueAndKey<Key, TypeData> 
{
    public event Action OnUpdateStorageLocal;
    
    //Жду пока все хранилеща будут инициализованы и только после этого вызываю инициализацую у этого общ. хран
    protected bool _isInit = false;
    public bool IsInit => _isInit;
    public event Action OnInit;
    
    [SerializeField] 
    private GetDataSO_SD_KeySwitherStorage _startKeyStorage;
    
    public SD_KeySwitherStorage CurrentKeySaveStorage => _currentKeySaveStorage;
    private SD_KeySwitherStorage _currentKeySaveStorage;

    [SerializeField] 
    private TypeStorage _currentGetSaveData;
    
    [SerializeField]
    private List<SaveDataStorageLocalKeyData<TypeStorage, Key, TypeData>> _listStorageLocal = new List<SaveDataStorageLocalKeyData<TypeStorage, Key, TypeData>>();

    private Dictionary<string, TypeStorage> _keyStorageLocal = new Dictionary<string, TypeStorage>();

    /// <summary>
    /// Дефолтные значения для переменных(будет установлены всем указанным хранилещам)
    /// </summary>
    [SerializeField] 
    private List<ClassDefaultKey> _defaultValueVariable = new List<ClassDefaultKey>();

    //private Dictionary<Key, Data> _keyDefaultValueVariable = new Dictionary<Key, Data>();
    
    
    protected void Awake()
    {
        foreach (var VARIABLE in _listStorageLocal)
        {
            _keyStorageLocal.Add(VARIABLE.Key.GetData().GetKey(), VARIABLE.Data); 
        }
        
        _currentGetSaveData = _keyStorageLocal[_startKeyStorage.GetData().GetKey()];
        _currentKeySaveStorage = _startKeyStorage.GetData();
        
        OnUpdateStorageLocal?.Invoke();
        
        _isInit = true;
        OnInit?.Invoke();
    }
    

    public bool EnableKeyInitData(SD_KeySwitherStorage keyAction)
    {
        foreach (var VARIABLE in _listStorageLocal)
        {
            if (VARIABLE.Key.GetData().GetKey() == keyAction.GetKey())
            {
                return true;
            }
        }

        return false;
    }

    public void SetStorageLocation(SD_KeySwitherStorage key, bool saveCurrentStorage = true)
    {
        _currentKeySaveStorage = key;

        if (saveCurrentStorage == true)
        {
            _currentGetSaveData.SaveData(new TaskInfo("Переключение хранилеща"));    
        }
        
        _currentGetSaveData = _keyStorageLocal[key.GetKey()];
        OnUpdateStorageLocal?.Invoke();
    }

    /// <summary>
    /// Скопирует все данные из хранилеща А в хранилеще Б
    /// </summary>
    public void CopyAllDataStorageLocal(SD_KeySwitherStorage getData, SD_KeySwitherStorage copyData, bool isSaveDataStorageCopy = true)
    {
        var listKey = _keyStorageLocal[getData.GetKey()].GetListKey();
        foreach (var VARIABLE in listKey)
        {
            _keyStorageLocal[copyData.GetKey()].SetData(VARIABLE,_keyStorageLocal[getData.GetKey()].GetData(VARIABLE));
        }

        if (isSaveDataStorageCopy == true)
        {
            _keyStorageLocal[copyData.GetKey()].SaveData(new TaskInfo("YY"));    
        }
    }

    /// <summary>
    /// Скопирует данные по указанным ключам
    /// </summary>
    public void CopyDataStorageLocal(List<Key> listKeyVariable,SD_KeySwitherStorage getData,SD_KeySwitherStorage copyData)
    {
        var listKey = listKeyVariable;
        foreach (var VARIABLE in listKey)
        {
            if (_keyStorageLocal[getData.GetKey()].IsThereData(VARIABLE) == true) 
            {
                _keyStorageLocal[copyData.GetKey()].SetData(VARIABLE,_keyStorageLocal[getData.GetKey()].GetData(VARIABLE));
            }
            else
            {
                Debug.Log("В базе " + _keyStorageLocal[getData.GetKey()] + " (по ключу  " + getData.GetKey() + " ) , не была найдена перменная (по ключу  " + VARIABLE + " )");
            }
            
        }
        
        _keyStorageLocal[copyData.GetKey()].SaveData(new TaskInfo("YY"));
    }

    public void AddStorage(SD_KeySwitherStorage key, TypeStorage storage)
    {
        _keyStorageLocal.Add(key.GetKey(), storage);
    }
    public void RemoveStorage(SD_KeySwitherStorage key)
    {
        _keyStorageLocal.Remove(key.GetKey());
    }

    public bool IsKeyStorage(SD_KeySwitherStorage key)
    {
        return _keyStorageLocal.ContainsKey(key.GetKey());
    }

    public TypeStorage GetCurrentStorage()
    {
        return _currentGetSaveData;
    }
    
    public TypeStorage GetStorage(SD_KeySwitherStorage key)
    {
        return _keyStorageLocal[key.GetKey()];
    }

    protected void OnSetCurrentStorageInspector()
    {
        if (_startKeyStorage != null)
        {
            if (_startKeyStorage.GetData() != null)
            {
                foreach (var VARIABLE in _listStorageLocal)
                {
                    if (VARIABLE.Key.GetData() == _startKeyStorage.GetData())
                    {
                        if (VARIABLE.Data != null)
                        {
                            _currentGetSaveData = VARIABLE.Data;
                        }
                    }
                } 
            }
        }
    }
}

[System.Serializable]
public class SaveDataStorageLocalKeyData<TypeStorage,K,D> where TypeStorage : SD_AbsStorage<K,D>
{
    public GetDataSO_SD_KeySwitherStorage Key;
    public TypeStorage Data;
}

