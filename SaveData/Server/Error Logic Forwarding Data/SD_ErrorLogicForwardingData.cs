using System;
using UnityEngine;


/// <summary>
/// Логика разрешения переотправки данных(запросов) к серверу
/// Наследники будут определять, сколько раз можно будет делать запросов на переотправку(или получения) данных
/// и при каких условиях(по мимо успешной отправки) будет сбрасываться счетчик(будет получено разрешение на продолжение отправки запросов) 
/// </summary>
public abstract class SD_ErrorLogicForwardingData : ScriptableObject
{
    public abstract event Action OnUpdateData;
    public abstract bool IsContinue { get; }


    public abstract void OnAddError();
    public abstract void OnRemoveAllError();
}
