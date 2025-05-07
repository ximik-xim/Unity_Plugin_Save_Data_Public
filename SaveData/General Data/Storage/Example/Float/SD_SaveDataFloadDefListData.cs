using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[System.Serializable]
public class SD_SaveDataFloadDefListData
{
    [SerializeField] 
    private List<AbsKeyData<string, float>> _list = new List<AbsKeyData<string, float>>();

    private void AddElement(string key, float data)
    {
        _list.Add(new AbsKeyData<string, float>(key, data));
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
    
    public float GetValue(string key)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].Key==key)
            {
                return _list[i].Data;
            }
        }

        return default;
    }

    public void SetValue(string key, float data)
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