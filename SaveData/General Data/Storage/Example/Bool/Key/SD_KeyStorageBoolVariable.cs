using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SD_KeyStorageBoolVariable 
{
    public SD_KeyStorageBoolVariable()
    {
        
    }
    
    public SD_KeyStorageBoolVariable(string key)
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
