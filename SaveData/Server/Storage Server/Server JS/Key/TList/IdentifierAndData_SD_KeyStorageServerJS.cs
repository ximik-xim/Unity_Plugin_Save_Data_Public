using UnityEngine; 
using TListPlugin; 
[System.Serializable]
public class IdentifierAndData_SD_KeyStorageServerJS : AbsIdentifierAndData<IndifNameSO_SD_KeyStorageServerJS, string, SD_KeyStorageServerJS>
{

 [SerializeField] 
 private SD_KeyStorageServerJS _dataKey;


 public override SD_KeyStorageServerJS GetKey()
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
  _dataKey = JsonUtility.FromJson<SD_KeyStorageServerJS>(json);
 }
#endif
}
