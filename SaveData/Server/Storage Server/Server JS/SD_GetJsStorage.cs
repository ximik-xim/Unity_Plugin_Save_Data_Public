using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_GetJsStorage
{
    private Func<string> _getJsMethod;
    
    public SD_GetJsStorage(Func<string> getJsMethod)
    {
        _getJsMethod = getJsMethod;
    }
    
    public string GetJSData()
    {
        return _getJsMethod();
    }
}
