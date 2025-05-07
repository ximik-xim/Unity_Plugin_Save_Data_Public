using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SD_SetServerDataJSWrapper : MonoBehaviour
{
    private bool _init = false;
    public bool IsInit => _init;
    public event Action OnInit;
    
    /// <summary>
    /// Нужен, если будет несколько таких классов для вызова покупок
    /// (у каждого экземпляра должен быть УНКИКАЛЬНЫЙ ключ)
    /// </summary>
    [SerializeField] 
    private string _keyInstanceClass = "";
    
    [SerializeField]
    private GetDKOPatch _patchStorageKey;
    [SerializeField]
    private GetDataSO_TSG_KeyStorageTask _keyStorageTaskBlock;
    private TSG_StorageTaskDefaultData _taskBlockStorage;

    /// <summary>
    /// Заблокированы ли проверка на куплен ли товар
    /// (может пригодиться в случе, если к серверу нужно отправить только 1 запрос и дождать его ответа, и нельзя
    /// в этот момент отправлять другой запрос) 
    /// </summary>
    public bool IsBlock => _taskBlockStorage.IsThereTasks();
    public event Action OnUpdateStatusBlock
    {
        add
        {
            _taskBlockStorage.OnUpdateStatus += value;
        }

        remove
        {
            _taskBlockStorage.OnUpdateStatus -= value;
        }
        
    }

    private Dictionary<int, SD_ServerRequestDataWrapperSetServerDataJS> _data = new Dictionary<int, SD_ServerRequestDataWrapperSetServerDataJS>();
    
    [SerializeField] 
    private SD_AbsSetServerDataJS _getServerDataJs;
    
    private void Awake()
    {
        if (_getServerDataJs.IsInit == false)
        {
            _getServerDataJs.OnInit += OnInitLogicBuy;
        } 
        
        if (_patchStorageKey.Init == false)
        {
            _patchStorageKey.OnInit += OnInitPatchStorageKey;
        } 
        
        CheckInit();
    }

    private void OnInitLogicBuy()
    {
        _getServerDataJs.OnInit -= OnInitLogicBuy;
        CheckInit();
    }
    
    private void OnInitPatchStorageKey()
    {
        _patchStorageKey.OnInit -= OnInitPatchStorageKey;
        CheckInit();
    }

    private void CheckInit()
    {
        if (_init == false)
        {
            if (_getServerDataJs.IsInit == true && _patchStorageKey.Init == true) 
            {
                Init();
            }    
        }
    }
    
    private void Init()
    {
        var storageKeyTaskDataMono= (DKODataInfoT<TSG_StorageKeyTaskDataMono>) _patchStorageKey.GetDKO();
        storageKeyTaskDataMono.Data.AddTaskData(_keyStorageTaskBlock.GetData(), new TSG_StorageTaskDefaultData());
        _taskBlockStorage = storageKeyTaskDataMono.Data.GetTaskData(_keyStorageTaskBlock.GetData());

        _init = true;
        OnInit?.Invoke();
    }


    /// <summary>
    /// Запрос данных на сервер
    /// 1 - pushDataJS это данные в виде JS, которые будут отправленны на сервер
    /// 2 - addDataJs это дополнительные данные для сервера в виде JS(не обяз., может быть пустышкой) пример, если нужно указать доп. инфу, откуда именно брать данные с сервера(из блока А или из блока Б, или все разом)
    /// </summary>
    /// <param name="addDataJs"></param>
    public GetServerRequestData<SD_DataSetRequestServerJSWrapperAddDataJS> SetServerDataJS(string pushDataJS, string addDataJs = "")
    {
        int id = 0;
        while (_data.ContainsKey(id) == true)
        {
            id = Random.Range(0, 2147483600);
        }
        
        var data = new SD_ServerRequestDataWrapperSetServerDataJS(id);
        _data.Add(id, data);

        _getServerDataJs.SetServerDataJS(Callback, id, addDataJs, _keyInstanceClass, pushDataJS);

        return data.DataGet;
    }


    /// <summary>
    ///     1 - id который отпровлял
    ///     2 - статус сервера
    ///     3 - сами данные которые отправлю назад
    /// </summary>
    private void Callback(int id, StatusCallBackServer statusServer, SD_DataSetRequestServerJSWrapperAddDataJS data, string keyInstanceClass)
    {
        if (keyInstanceClass == _keyInstanceClass)
        {
            var dataReturn = _data[id].Data;

            dataReturn.IsGetDataCompleted = true;
            dataReturn.StatusServer = statusServer;
            dataReturn.GetData = data;
            
            _data.Remove(id);

            dataReturn.Invoke();
        }
    }

    /// <summary>
    /// Есть ли запросы на обработку этого Id
    /// </summary>
    public bool IsProductStartWating(int id)
    {
        if (_data.ContainsKey(id) == true) 
        {
            return true;
        }

        return false;
    }
    
}
