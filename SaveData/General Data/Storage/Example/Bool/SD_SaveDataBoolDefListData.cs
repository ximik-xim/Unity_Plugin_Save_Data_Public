using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SD_SaveDataBoolDefListData 
{
    [SerializeField] 
    private List<AbsKeyData<string, bool>> _list = new List<AbsKeyData<string, bool>>();

    private void AddElement(string key, bool data)
    {
        _list.Add(new AbsKeyData<string, bool>(key, data));
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
    
    public bool GetValue(string key)
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

    public void SetValue(string key, bool data)
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
