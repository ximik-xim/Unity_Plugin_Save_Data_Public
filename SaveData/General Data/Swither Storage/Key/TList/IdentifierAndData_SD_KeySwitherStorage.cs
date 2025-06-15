using UnityEngine; 
using TListPlugin; 
[System.Serializable]
public class IdentifierAndData_SD_KeySwitherStorage : AbsIdentifierAndData<IndifNameSO_SD_KeySwitherStorage, string, SD_KeySwitherStorage>
{

 [SerializeField] 
 private SD_KeySwitherStorage _dataKey;


 public override SD_KeySwitherStorage GetKey()
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
  _dataKey = JsonUtility.FromJson<SD_KeySwitherStorage>(json);
 }
#endif
}
