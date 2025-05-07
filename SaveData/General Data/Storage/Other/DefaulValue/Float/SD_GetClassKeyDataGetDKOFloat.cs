using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class SD_GetClassKeyDataGetDKOFloat : SD_AbsDefaultValueAndKey<SD_KeyStorageFloatVariable,float>
{
    [SerializeField]
    private GetDataSO_SD_KeyStorageFloatVariable _getDataSoSaveDataFloat;
    
    public override SD_KeyStorageFloatVariable GetKey()
    {
        return _getDataSoSaveDataFloat.GetData();
    }
    
}
