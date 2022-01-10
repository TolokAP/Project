using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class ItemDB : MonoBehaviour
    {

        public List<ItemData> itemDatabase;

    }
    public enum TypeEquipment { none, weapon, twoHandWeapon, helmet,chest,legs,gloves,earrings,back };//тип предмета, для экипировки

   
    public enum AttackDistance // Дистанция атаки для разных классов. Список.
    {
        None = 0,
        Melee = 2,
        Range = 20,
        Magic = 10
    }
    [System.Serializable]
    public class ItemData
    {
        //stackable, stack size, graphic, type, value, description, stat1, stat2, etc

        public int ID;
        public string name;
        public GameObject prefab;
        public Sprite icon;
        public bool stackable;
        public int stackSize;
        public string description;
        public CombatsSkills skill;
        public TypeEquipment type;
        public AttackDistance attackDistance;
        public int[] damage = new int[2];
        public float value;
        public SpecialAttack specialAttack;
        public float armor;
        public float stat3;
        public int materialID;
        public int equipID;
    }
}