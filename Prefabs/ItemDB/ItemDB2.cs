using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
  
    public class ItemDB2 : ScriptableObject
    {
        public List<Human> humen;
       
        public void GetName()
        {

        }
    }
    [System.Serializable]
    public class Human
    {
        public string _name;

        public int ID;
        public int GetID
        {
            get { return ID; }
        }

    }
    
    
   
}