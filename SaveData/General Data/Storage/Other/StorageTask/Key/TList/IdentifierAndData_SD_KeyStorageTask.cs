using UnityEngine; 
using TListPlugin; 
[System.Serializable]
public class IdentifierAndData_SD_KeyStorageTask : AbsIdentifierAndData<IndifNameSO_SD_KeyStorageTask, string, SD_KeyStorageTask>
{

 [SerializeField] 
 private SD_KeyStorageTask _dataKey;


 public override SD_KeyStorageTask GetKey()
 {
  return _dataKey;
 }
}
