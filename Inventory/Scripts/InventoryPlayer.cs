
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Player
{
    public class InventoryPlayer : GlobalEventListener
    {
        public List<GameObject> mySlots;

        public ItemDB ItemDB;

        public GameObject selectedSlot;
        public GameObject slot;
        public GameObject _item;
        public GameObject slotposition;
        public InventoryPlayerController PlayerController;
        public GameObject infoItemPrefab;
        public GameObject _infoItem;

        public List<GameObject> myEquipment = new List<GameObject>(8);
        public Dictionary<SpecialAttack, string> abilityDiscription;


        public void Start()
        {
            abilityDiscription = new Dictionary<SpecialAttack, string>
            {
                { SpecialAttack.doubleDamage, "Двойной урон - по вашей текущей целе" },
                { SpecialAttack.stun, "Оглушение цели на 5 секунд" },
                { SpecialAttack.activeShield, "Щит - блокирует весь урон на 7 секунд" },
                 { SpecialAttack.none, "-" }
            };
        }
        #region Методы управления информацией по предмету
        public void OpenWindowItemInfo(Transform parent,int slotId) //Создание информационного окна по предмету
        {
            BoltLog.Warn("Сработал метод открытия инфо");
            
                CloseWindowItemInfo();
                _infoItem = Instantiate(infoItemPrefab, parent.transform);
                _infoItem.transform.SetParent(gameObject.transform);
                _infoItem.GetComponent<InfoItem>().slotID = slotId;
                GetInfoItem(slotId);

                    
        }

        public void CloseWindowItemInfo()//Удаление информационного окна по предмету
        {


            if (_infoItem)
            {
                Destroy(_infoItem);
            }
        }

        private void GetInfoItem(int slotId)//Получение информации  о предмете и передача информации вновь созданному информационному окну
        {
            int Id = PlayerController.boltEntity.GetState<IPlayer>().items[slotId].ID;
            ItemData itemData = lookUpID(Id);
            string _itemDamageinfo;
            switch (itemData.type) // Выбор надписи в зависимости от того какого типа предмет
            {
                case TypeEquipment.weapon:
                    _itemDamageinfo = "Урон: " + itemData.damage[0] + " - " + itemData.damage[1];
                    break;
                default:
                    _itemDamageinfo = "Броня: " + itemData.armor;
                    break;

            }

           
            _infoItem.GetComponent<InfoItem>().SetStringInfo(itemData.description, _itemDamageinfo,abilityDiscription[itemData.specialAttack]);//Передача вновь созданному предмету параметров для отображения



        }


        #endregion



        public ItemData lookUpID(int ID)
        {
            foreach (ItemData ItemData in ItemDB.itemDatabase)
            {
                if (ItemData.ID == ID)
                {
                    return ItemData;
                }
            }
            return null;

        }
        public bool returnType(TypeEquipment type,int slot)//Проверка на тип слота в экипировке
        {
            if (type==myEquipment[slot].GetComponent<InventoryDrop>().typeEquipment) return true;
            else return false;


        } 

      

        public void instantiateEquipmentSlot()//Получение слотов экипировки в myEquipment
        {

            for (int i = 0; i < myEquipment.Count; i++)
            {
                myEquipment[i].GetComponent<InventoryDrop>().slot = i;
            }


        }

        public void instantiateSlot(Vector3 position, int ID)
        {
            GameObject newSlot = Instantiate(slot);
            newSlot.GetComponent<InventoryDrop>().slot = ID;
            newSlot.transform.SetParent(slotposition.transform);
            newSlot.GetComponent<RectTransform>().localPosition = Vector3.zero;
            newSlot.GetComponent<RectTransform>().localScale = Vector3.one;
            mySlots.Add(newSlot);


        }



        public void instantiateSlots(int count)
        {
            BoltLog.Warn("Создаем слоты инвентарь");
            mySlots.Clear();
            foreach (Transform child in slotposition.transform)
            {
                GameObject.Destroy(child.gameObject);
            }


            for (int i = 0; i < count; i++)
            {

                instantiateSlot(Vector3.zero, i);

                if (i == 0)
                    EventSystem.current.SetSelectedGameObject(mySlots[0]);
            }


            instantiateEquipmentSlot();
            refreshItem(mySlots, PlayerController.boltEntity.GetState<IPlayer>().items);
            refreshItem(myEquipment, PlayerController.boltEntity.GetState<IPlayer>().Equipmnet);
        }
        public void instantiateItem(int slotID, int ID, int quantity, List<GameObject> ListObject)
        {

            if (ListObject.Count <= slotID)
                return; //doesnt exist

            //clear the children
            foreach (Transform child in ListObject[slotID].transform)
            {if(child.GetComponent<InventoryDrag>())
                Destroy(child.gameObject);
            }

            //check if slot exists and is available (on server, this is just ui logic)

            GameObject newItem = Instantiate(_item);
            ItemData ItemData = lookUpID(ID);
            if (ItemData != null)
            {
                if (ItemData.icon != null)
                    newItem.GetComponent<Image>().sprite = ItemData.icon;
            }

            if (quantity > 1)
                newItem.transform.GetChild(0).GetComponent<Text>().text = quantity.ToString();
            newItem.GetComponent<InventoryDrag>().slot = slotID;
            newItem.transform.SetParent(ListObject[slotID].transform);
            newItem.GetComponent<RectTransform>().localPosition = Vector3.zero;
            newItem.GetComponent<RectTransform>().localScale = Vector3.one;


        }
        


        public void refreshItem(List<GameObject> gameObjects, NetworkArray_Objects<item> myState)
        {
            GameObject PAL = GameObject.FindGameObjectWithTag("Player");
            if (PAL.GetComponent<BoltEntity>().IsOwner)
            {
               
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                       if (myState[i].ID != 0)
                        {
                            instantiateItem(i, myState[i].ID, myState[i].quantity,gameObjects);
                         
                        }
                        else
                        {
                        
                            
                            foreach (Transform child in gameObjects[i].transform)
                            {
                                if (child.GetComponent<InventoryDrag>()) Destroy(child.gameObject);
                            }
                        }
                    }

            }
        }

       

    }
}
