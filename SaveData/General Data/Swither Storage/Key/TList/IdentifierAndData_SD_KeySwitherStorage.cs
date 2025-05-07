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
}
