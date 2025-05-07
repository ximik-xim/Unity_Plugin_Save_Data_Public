using UnityEngine; 
using TListPlugin; 
[System.Serializable]
public class IdentifierAndData_SD_KeyStorageFloatVariable : AbsIdentifierAndData<IndifNameSO_SD_KeyStorageFloatVariable, string, SD_KeyStorageFloatVariable>
{

 [SerializeField] 
 private SD_KeyStorageFloatVariable _dataKey;


 public override SD_KeyStorageFloatVariable GetKey()
 {
  return _dataKey;
 }
}
