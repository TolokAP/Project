using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class PLayer:BasePlayer
    {
       
        public string Login { get; set; }
     
        public float[] combatSkills = new float[3];
        public int[,] inventory = new int[5,2];
        public int[] equipment = new int[8];

        public int[] stats = new int[4];
        public int Level { get; set; }
        public int Exp { get; set; }
        public int[] damage = new int[2];

        public void SetSkill(int i,float value)
        {
            combatSkills[i] = value;
        }


    }
   
   
    public enum StatsPlayer { Strengh, Agility, Stamina, Power, }// Параметр персонажа
}