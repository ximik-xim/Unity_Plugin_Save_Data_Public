using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SD_GetClassKeyDataGetDKOBool : SD_AbsDefaultValueAndKey<SD_KeyStorageBoolVariable,bool>
{
    [SerializeField]
    private GetDataSO_SD_KeyStorageBoolVariable _getDataSoSaveDataBool;
    
    public override SD_KeyStorageBoolVariable GetKey()
    {
        return _getDataSoSaveDataBool.GetData();
    }
}
