using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SD_KeyStorageStringVariable 
{
    public SD_KeyStorageStringVariable()
    {
        
    }
    
    public SD_KeyStorageStringVariable(string key)
    {
        _key = key;
    }
    
    [SerializeField]
    private string _key;

    public string GetKey()
    {
        return _key;
    }
}
