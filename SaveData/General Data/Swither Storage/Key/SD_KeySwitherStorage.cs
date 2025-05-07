using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SD_KeySwitherStorage : IGetKey<string>
{
    [SerializeField]
    private string key;

    public string GetKey()
    {
        return key;
    }
}
