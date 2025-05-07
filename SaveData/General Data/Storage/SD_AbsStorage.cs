using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SD_AbsStorage<Key,Data> : ScriptableObject
{
    /// <summary>
    /// event срабатывающий когда хранилище было инициализировано( при инициализации данные не подргужаються, их нужно отдельно подгружать самому, с помощью метода UploadingData(любого))
    /// </summary>
    public abstract event Action OnInit;
    
    /// <summary>
    ///  event срабатывающий когда хранилище пришли данные для выгрузки (актуально для общение с серверами)
    /// </summary>
    public abstract event Action OnUpdateData;
    
    /// <summary>
    ///  event срабатывающий когда пришел ответ от сервера (актуально для общение с серверами)
    /// </summary>
    public abstract event Action OnLastStatusUpdateData;
    /// <summary>
    /// Статус последней попытки выгрузить данные (актуально для общение с серверами)
    /// </summary>
    public abstract StatusStorageAction LastStatusUpdateData { get; }
    
    /// <summary>
    ///  event срабатывающий когда было изменения значение переменной с помощью Set (Не сработыет при выгрузке данных, для этого есть OnUpdateData)
    /// </summary>
    public abstract event Action<Key> OnUpdateValue;

    /// <summary>
    ///  event срабатывающий когда было сохранение данных (актуально для общение с серверами) 
    /// </summary>
    public abstract event Action OnSaveDataComplited; 
    
    /// <summary>
    /// Статус последней попытки сохранить данные (актуально для общение с серверами)
    /// </summary>
    public abstract StatusStorageAction LastStatusSaveData { get; }
    
    /// <summary>
    /// Вызовет инициализацию хранилища
    /// </summary>
    public abstract void InitializationStorage();
    
    /// <summary>
    /// Была ли инициализация
    /// </summary>
    public abstract bool IsInit { get; } 
    
    /// <summary>
    /// Есть ли данные по ключу
    /// </summary>
    /// <param name="dataKey"></param>
    /// <returns></returns>
    public abstract bool IsThereData(Key dataKey);
    
    /// <summary>
    /// Получить данные по ключу
    /// </summary>
    /// <param name="dataKey"></param>
    /// <returns></returns>
    public abstract Data GetData(Key dataKey);
    
    /// <summary>
    /// Установить данные по ключу
    /// </summary>
    /// <param name="dataKey"></param>
    /// <param name="data"></param>
    public abstract void SetData(Key dataKey, Data data);

    /// <summary>
    /// Добавит Task показывающую, что нужно сохранить данные, при первой возможности
    /// Если в аргументе срочности указать True, то будут проигнорированы обычные Task на ожидание(использовать в крайнех случаях)
    /// </summary>
    public abstract void SaveData(TaskInfo taskInfo, bool urgentSaving = false);

    /// <summary>
    /// Добавит Task показывающую, что нужно выгрузить данные, при первой возможности
    /// Если в аргументе срочности указать True, то будут проигнорированы обычные Task на ожидание(использовать в крайнех случаях)
    /// </summary>
    public abstract void UploadingData(TaskInfo taskInfo, bool urgentUploading = false);
    
    /// <summary>
    /// Вернет список ключей переменных, которые есть в хранилище
    /// (конечно спорное решение, но лучше пусть будет)
    /// </summary>
    /// <returns></returns>
    public abstract IReadOnlyList<Key> GetListKey();

    /// <summary>
    /// Запущен ли запрос на выгрузку данных(актуально для общение с серверами)
    /// </summary>
    /// <returns></returns>
    public abstract bool IsWaitingResponseSaveData();
    
    /// <summary>
    /// Запущен ли запрос на сохранение данных(актуально для общение с серверами)
    /// </summary>
    /// <returns></returns>
    public abstract bool IsWaitingResponseUploadingData();
    
    /// <summary>
    /// Добавить дефолтное значение переменной(если ее нету в хранилеще данных, то вернет дефолтное значение)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    public abstract void AddDefValue(Key key, Data data);
    /// <summary>
    /// Вернет значение переменной
    /// </summary>
    /// <param name="dataKey"></param>
    /// <returns></returns>
    public abstract Data GetDefValue(Key dataKey);
    
    /// <summary>
    ///  Есть ли данные дефолтное значение по ключу 
    /// </summary>
    /// <param name="dataKey"></param>
    /// <returns></returns>
    public abstract bool IsThereDataDef(Key dataKey);

    /// <summary>
    /// Нужен что бы можно было напрямую у хранилеща добавлять или удалять Task блокировку сохранения
    /// </summary>
    public abstract SD_StorageTask GetStorageTaskBlockSave();

    /// <summary>
    /// Нужен что бы можно было напрямую у хранилеща добавлять или удалять Task блокировку выгрузки данных(с сервера или еще откудо то)
    /// </summary>
    public abstract SD_StorageTask GetStorageTaskBlockUploading();

}

public enum StatusStorageAction
{
    None,
    Error,
    Ok
}

