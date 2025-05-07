using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Нужен для возможности получать хранилеще String разными путями
/// </summary>
public abstract class SD_AbsGetStorageStringProduct : MonoBehaviour
{
    public abstract event Action OnInit;
    public abstract bool IsInit { get; }

    public abstract SD_AbsStringStorage GetStorage();
}
