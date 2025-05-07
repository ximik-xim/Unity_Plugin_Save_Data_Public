using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SD_GetClassKeyDataGetDKOString : SD_AbsDefaultValueAndKey<SD_KeyStorageStringVariable,string>
{
    [SerializeField]
    private GetDataSO_SD_KeyStorageStringVariable  _getDataSoSaveDataString;
    
    public override SD_KeyStorageStringVariable GetKey()
    {
        return _getDataSoSaveDataString.GetData();
    }
}
