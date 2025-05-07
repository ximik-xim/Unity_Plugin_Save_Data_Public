using System;
using UnityEngine;

/// <summary>
/// Нужен что бы сбрасывать счетчик ошибок при запуске проекта
/// (в каждом проекте это будет уникальноая логика)
/// </summary>
public class ResetCounterErrorServer : MonoBehaviour
{
   [SerializeField] 
   private SD_ErrorLogicForwardingDataCouner _errorCounter;
   
   private void Awake()
   {
      _errorCounter.RemoveCount();
   }
}
