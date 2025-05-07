using System;
using UnityEngine;

public abstract class SD_AbsGetServerDataJS : MonoBehaviour
{
    public abstract bool IsInit { get; }
    public abstract event Action OnInit;

    public abstract void GetServerDataJS(Action<int, StatusCallBackServer, SD_DataGetRequestServerJSWrapperAddDataJS, string> callback, int id, string addDataJs, string keyInstanceClass);
}
