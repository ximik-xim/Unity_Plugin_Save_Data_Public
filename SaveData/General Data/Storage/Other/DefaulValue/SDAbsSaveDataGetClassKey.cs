using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Нужно для возможности указания дефолтное значения через инспектор
/// </summary>
public abstract class SD_AbsDefaultValueAndKey<Key,Data>:IGetKey<Key>
{
    [SerializeField]
    private Data _defaulValue;
    
    public Data DefaulValue => _defaulValue;

    public abstract Key GetKey();

}