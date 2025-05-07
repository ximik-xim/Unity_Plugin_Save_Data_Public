using UnityEngine; 
using TListPlugin; 
[System.Serializable]
public class IdentifierAndData_SD_KeyStorageBoolVariable : AbsIdentifierAndData<IndifNameSO_SD_KeyStorageBoolVariable, string, SD_KeyStorageBoolVariable>
{

 [SerializeField] 
 private SD_KeyStorageBoolVariable _dataKey;


 public override SD_KeyStorageBoolVariable GetKey()
 {
  return _dataKey;
 }
}
