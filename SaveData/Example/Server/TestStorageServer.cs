using UnityEngine;

public class TestStorageServer : MonoBehaviour
{
    [SerializeField] 
      private SD_AbsFloatStorage _storage;
   
      [SerializeField] 
      private GetDataSO_SD_KeyStorageFloatVariable _keyStorage;
   
      [SerializeField] 
      private float _saveValue;
   
      [SerializeField] 
      private bool _addTaskBlockUploading;
      
      [SerializeField] 
      private bool _addTaskBlockSave;
      private void Awake()
      {
         if (_storage.IsInit == false)
         {
            _storage.OnInit += OnInitStorage;
            return;
         }
   
         InitStorage();
      }
   
      private void OnInitStorage()
      {
         _storage.OnInit -= OnInitStorage;
         InitStorage();
      }
      
      private void InitStorage()
      {
         var keyTaskBlockUploading = new SD_KeyStorageTask("test");
         if (_addTaskBlockUploading == true)
         {
            _storage.GetStorageTaskBlockUploading().AddTask(keyTaskBlockUploading,"test");
         }

         _storage.OnUpdateData += OnUpdateData;
         Debug.Log("Запрос на выгрузку данных");
         _storage.UploadingData(new TaskInfo("test"));
   
         if (_addTaskBlockUploading == true)
         {
            Debug.Log("Снятие блокировки с выгрузки данных");
            _storage.GetStorageTaskBlockUploading().RemoveTask(keyTaskBlockUploading);
         }
         
      }

      private void OnUpdateData()
      {
         if (_storage.LastStatusUpdateData == StatusStorageAction.Ok)
         {
            _storage.OnUpdateData -= OnUpdateData;
         
            var data = _storage.GetData(_keyStorage.GetData());
            Debug.Log("Данные в хранилеще = " + data);   
         }

         StartSaveData();
      }

      private void StartSaveData()
      {
         var keyTaskBlockSave = new SD_KeyStorageTask("test");
         if (_addTaskBlockSave == true)
         {
            _storage.GetStorageTaskBlockSave().AddTask(keyTaskBlockSave,"test");
         }

         _storage.OnSaveDataComplited += OnSaveData;
         
         Debug.Log("Запрос на сохранение данных");
         _storage.SetData(_keyStorage.GetData(), _saveValue);
         _storage.SaveData(new TaskInfo("Тестовое сохранение"));
         
         if (_addTaskBlockSave == true)
         {
            Debug.Log("Снятие блокировки с сохранение данных");
            _storage.GetStorageTaskBlockSave().RemoveTask(keyTaskBlockSave);
         }
      }
      
      private void OnSaveData()
      {
         if (_storage.LastStatusSaveData == StatusStorageAction.Ok)
         {
            _storage.OnSaveDataComplited -= OnSaveData;

            Debug.Log("Данные были успешно сохранены");
         }
      }
}
