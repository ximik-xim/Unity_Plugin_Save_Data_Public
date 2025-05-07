
using System;
using UnityEngine;

public abstract class SD_AbsSetServerDataJS : MonoBehaviour
{
    public abstract bool IsInit { get; }
    public abstract event Action OnInit;

    public abstract void SetServerDataJS(Action<int, StatusCallBackServer, SD_DataSetRequestServerJSWrapperAddDataJS, string> callback, int id, string addDataJs, string keyInstanceClass, string pushDataJS);
}
