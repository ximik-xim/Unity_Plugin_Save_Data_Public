using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SD_KeyStorageFloatVariable : IGetKey<string>
{
    public SD_KeyStorageFloatVariable()
    {
        
    }
    
    public SD_KeyStorageFloatVariable(string key)
    {
        this.key = key;
    }
    
    [SerializeField]
    private string key;

    public string GetKey()
    {
        return key;
    }
}
