using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SD_SaveDataStringDefListData 
{
    [SerializeField] 
    private List<AbsKeyData<string, string>> _list = new List<AbsKeyData<string, string>>();

    private void AddElement(string key, string data)
    {
        _list.Add(new AbsKeyData<string, string>(key, data));
    }

    public void RemoveElement(string key)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].Key==key)
            {
                _list.RemoveAt(i);
                return;
            }
        }
    }

    public bool IsThereData(string key)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].Key==key)
            {
                return true;
            }
        }

        return false;
    }
    
    public string GetValue(string key)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].Key == key) 
            {
                return _list[i].Data;
            }
        }

        return default;
    }

    public void SetValue(string key, string data)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].Key==key)
            {
                _list[i].Data = data;
                return;
            }
        }

        AddElement(key, data);
    }

    public IReadOnlyList<string> GetListKey()
    {
        List<string> listKey = new List<string>();
        foreach (var VARIABLE in _list)
        {
            listKey.Add(VARIABLE.Key);
        }

        return listKey;
    }
}
