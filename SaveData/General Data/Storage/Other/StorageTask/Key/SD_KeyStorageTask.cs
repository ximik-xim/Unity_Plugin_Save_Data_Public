using UnityEngine;

[System.Serializable]
public class SD_KeyStorageTask : IGetKey<string>
{
    public SD_KeyStorageTask(string key)
    {
        _key = key;
    }
    
    public SD_KeyStorageTask()
    {
        
    }
    
    [SerializeField]
    private string _key;

    public string GetKey()
    {
        return _key;
    }
}
