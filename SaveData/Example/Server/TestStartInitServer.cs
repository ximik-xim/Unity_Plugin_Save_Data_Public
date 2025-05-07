using System;
using UnityEngine;

public class TestStartInitServer : MonoBehaviour
{
   [SerializeField] 
   private SD_ServerStorageDataLogicJS _serverStorage;
   
   [SerializeField] 
   private SD_AbsFloatStorage _storage;
   
   private void Awake()
   {
      if (_serverStorage.IsInit == false)
      {
         _serverStorage.InitializationStorage();   
      }
      
      if (_storage.IsInit == false)
      {
         _storage.InitializationStorage();   
      }
   }
}
