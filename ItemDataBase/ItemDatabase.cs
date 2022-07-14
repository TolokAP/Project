using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player { 
[CreateAssetMenu(menuName = "Databases/Item", fileName = "Item")]

public class ItemDatabase : ScriptableObject
{
    public List<ItemData> _itemData;
    [SerializeField] private List<ItemData> _currentListItemData;
    [SerializeField] private ItemData newItem;
  


   

    public void AddElement()
    {
        if (_itemData == null)
            _itemData = new List<ItemData>();
        newItem = new ItemData();
        newItem.GetIDItem = _itemData.Count+1;
        _itemData.Add(newItem);
       
      

    }

    public void SortListAllItem()
    {
        if (_currentListItemData == null)
            _currentListItemData = new List<ItemData>();

        _currentListItemData.Clear();
        foreach (ItemData itemData in _itemData)
        {
            _currentListItemData.Add(itemData);
        }


    }
    public void SortList(TypeEquipment typeEquipment)
    {
        if (_currentListItemData == null)
            _currentListItemData = new List<ItemData>();

        _currentListItemData.Clear();

        foreach (ItemData itemData in _itemData)
        {
            if (itemData.GetTypeEquipment == typeEquipment) _currentListItemData.Add(itemData);
        }

    }


    public void SaveInfoItem()
    {

        switch (newItem.GetTypeEquipment)
        {
            case TypeEquipment.helmet:
                newItem.GetEquiplID = 0;
                break;
            case TypeEquipment.chest:
                newItem.GetEquiplID = 1;
                break;
            case TypeEquipment.legs:
                newItem.GetEquiplID = 2;
                break;
            case TypeEquipment.gloves:
                newItem.GetEquiplID = 5;
                break;
            case TypeEquipment.earrings:
                newItem.GetEquiplID = 6;
                break;
            case TypeEquipment.back:
                newItem.GetEquiplID = 7;
                break;
        }
      
        newItem = new ItemData();

        
    }

    public ItemData LookIDItem(int ID)
    {
        foreach(ItemData itemData in _itemData)
        {
           
            if (itemData.GetIDItem == ID)
            {
                return itemData;
            }
        }
        return null;
    }

}

public enum TypeItem
{ 
   none,
   weapon,
   armor,

}
    public enum TypeMaterial
    {
        Iron,
        Silver,
        Gold

    }
public enum TypeEquipment
{
    helmet,
    chest,
    legs,
    weapon,
    twoHandWeapon,
    gloves, 
    earrings,
    back,
    none,
    }
public enum CombatsSkills 
{
    Archery,
    Swordsmanship,
    Fencing,
    Parring 
}

public enum AttackDistance // ��������� ����� ��� ������ �������. ������.
{
    None = 0,
    Melee = 2,
    Range = 20,
    Magic = 10
}

public enum SpecialAttack
{
    none,
    doubleDamage,
    activeShield,
    stun,



}


    [System.Serializable]
    public class ItemData
    {
        [Tooltip("��� ��������")]
        [SerializeField] private string _name;

        public string GetNameItem
        {
            get => _name;
        }

        [Tooltip("�������� ��������")]
        [SerializeField] private string _discription;
        public string GetDiscription
        {
            get => _discription;
        }

        [Tooltip("ID ��������")]
        [SerializeField] private int _id;
        public int GetIDItem
        {
            get => _id;
            set => _id = value;
        }
        [Tooltip("��� ��������")]
        [SerializeField] private TypeItem _typeItem;
        public TypeItem GetTypeItem
        {
            get => _typeItem;
        }
        [Tooltip("��� �������� ��� ����������")]
        [SerializeField] private TypeEquipment _typeEquipment;
        public TypeEquipment GetTypeEquipment
        {
            get => _typeEquipment;
        }

        [Tooltip("ID ����� � ����������")]
        private int _equipID;
        public int GetEquiplID
        {
            get => _equipID;
            set => _equipID = value;
        }

        [Tooltip("������ ��������")]
        [SerializeField] private GameObject _prefabItem;
        public GameObject GetPrefabItem
        {
            get => _prefabItem;
        }
        [Tooltip("������ ��������")]
        [SerializeField] private Sprite _iconItem;
        public Sprite GetIconItem
        {
            get => _iconItem;
        }

        [Tooltip("��������� ��������")]
        [SerializeField] private bool _stackable;
        public bool GetItemStackable
        {
            get => _stackable;

        }

        [Tooltip("������ ��� ����������")]
        [SerializeField] private int _stackSize;
        public int GetStackSize
        {
            get => _stackSize;

        }

        [Tooltip("������ ������")]
        [SerializeField] private CombatsSkills _combatSkill;
        public CombatsSkills GetCombatsSkill
        {
            get => _combatSkill;

        }

        [Tooltip("����������� �����")]
        [SerializeField] private SpecialAttack _specialAttack;
        public SpecialAttack GetSpecialAttack
        {
            get => _specialAttack;

        }

        [Tooltip("��������� �����")]
        [SerializeField] private AttackDistance _attackDistance;
        public AttackDistance GetAttackDistance
        {
            get => _attackDistance;

        }

        [Tooltip("��� ����� ������")]
        [SerializeField] private int _minDamageWeapon;
        public int GetMinDamageWeapon
        {
            get => _minDamageWeapon;

        }

        [Tooltip("���� ����� ������")]
        [SerializeField] private int _maxDamageWeapon;
        public int GetMaxDamageWeapon
        {
            get => _maxDamageWeapon;

        }

        [Tooltip("��������� ��������")]
        [SerializeField] private int _valueItem;
        public int GetValueItem
        {
            get => _valueItem;

        }

        [Tooltip("����� ��������")]
        [SerializeField] private int _armorItem;
        public int GetArmorItem
        {
            get => _armorItem;

        }

        [Tooltip("ID ���������")]
        [SerializeField] private TypeMaterial _typeMaterial;
        public TypeMaterial GetTypeMaterial
        {
            get => _typeMaterial;

        }



    }

}
