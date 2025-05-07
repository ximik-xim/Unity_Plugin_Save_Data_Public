using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Нужен для возможности получать хранилеще Float разными путями
/// </summary>
public abstract class SD_AbsGetStorageFloatProduct : MonoBehaviour
{
    public abstract event Action OnInit;
    public abstract bool IsInit { get; }

    public abstract SD_AbsFloatStorage GetStorage();
}
