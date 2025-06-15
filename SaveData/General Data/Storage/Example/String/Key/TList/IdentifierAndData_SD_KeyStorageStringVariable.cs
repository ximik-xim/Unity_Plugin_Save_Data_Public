using UnityEngine; 
using TListPlugin; 
[System.Serializable]
public class IdentifierAndData_SD_KeyStorageStringVariable : AbsIdentifierAndData<IndifNameSO_SD_KeyStorageStringVariable, string, SD_KeyStorageStringVariable>
{

 [SerializeField] 
 private SD_KeyStorageStringVariable _dataKey;


 public override SD_KeyStorageStringVariable GetKey()
 {
  return _dataKey;
 }
 
#if UNITY_EDITOR
 public override string GetJsonSaveData()
 {
  return JsonUtility.ToJson(_dataKey);
 }

 public override void SetJsonData(string json)
 {
  _dataKey = JsonUtility.FromJson<SD_KeyStorageStringVariable>(json);
 }
#endif
}
