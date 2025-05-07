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
}
