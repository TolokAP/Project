using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Bolt.Utils;
using UnityEngine.EventSystems;
using Photon.Bolt;


namespace Player{

    public class ShopInfoItem : MonoBehaviour, IPointerClickHandler
    {
        private int _id;
        private ItemDB _ItemDB;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _infoItem;
        [SerializeField] private Text _ability;
        [SerializeField] private InventoryPlayerController _inventoryPlayerController;
        private ItemData _itemData;



        private void Start()
        {
            _ItemDB = GameObject.Find("ItemDB").GetComponent<ItemDB>();
            _itemData = lookUpID(_id);
            _icon.sprite = _itemData.icon;
            _name.text = _itemData.name;
            _ability.text = _itemData.specialAttack.ToString();
        }
        
        public ItemData lookUpID(int ID)
        {
            foreach (ItemData ItemData in _ItemDB.itemDatabase)
            {
                if (ItemData.ID == ID)
                {
                    return ItemData;
                }
            }
            return null;

        }

        public void SetID(int id)
        {
            _id = id;
        }




        public void OnPointerClick(PointerEventData eventData)
        {
            if(_inventoryPlayerController==null){
                _inventoryPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryPlayerController>();
            }
           _inventoryPlayerController.addItem(_id,1);
             
        }

    }
}