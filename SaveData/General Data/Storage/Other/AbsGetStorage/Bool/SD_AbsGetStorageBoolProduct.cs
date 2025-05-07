using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Нужен для возможности получать хранилеще Bool разными путями
/// </summary>
public abstract class SD_AbsGetStorageBoolProduct : MonoBehaviour
{
    public abstract event Action OnInit;
    public abstract bool IsInit { get; }

    public abstract SD_AbsBoolStorage GetStorage();
}
