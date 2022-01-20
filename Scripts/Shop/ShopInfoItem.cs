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
        public ItemDatabase itemDatabase;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _infoItem;
        [SerializeField] private Text _ability;
        [SerializeField] private InventoryPlayerController _inventoryPlayerController;
        private ItemData _itemData;



        private void Start()
        {
           
            _itemData = itemDatabase.LookIDItem(_id);
            _icon.sprite = _itemData.GetIconItem;
            _name.text = _itemData.GetDiscription;
            _ability.text = _itemData.GetSpecialAttack.ToString();
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