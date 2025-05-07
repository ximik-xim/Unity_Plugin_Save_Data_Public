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
}
