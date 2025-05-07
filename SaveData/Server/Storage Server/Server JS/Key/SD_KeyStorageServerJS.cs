using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SD_KeyStorageServerJS 
{
   [SerializeField]
   private string _key;

   public string GetKey()
   {
      return _key;
   }
}
