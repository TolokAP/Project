
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

        public ItemDatabase ItemDatabase;


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
                { SpecialAttack.doubleDamage, "������� ���� - �� ����� ������� ����" },
                { SpecialAttack.stun, "��������� ���� �� 5 ������" },
                { SpecialAttack.activeShield, "��� - ��������� ���� ���� �� 7 ������" },
                 { SpecialAttack.none, "-" }
            };
           

        }
    

       
        #region ������ ���������� ����������� �� ��������
        public void OpenWindowItemInfo(Transform parent,int IDItem) //�������� ��������������� ���� �� ��������
        {
            BoltLog.Warn("�������� ����� �������� ����");
            
                CloseWindowItemInfo();
                _infoItem = Instantiate(infoItemPrefab, parent.transform);
                _infoItem.transform.SetParent(gameObject.transform);
                GetInfoItem(IDItem);

                    
        }

        public void CloseWindowItemInfo()//�������� ��������������� ���� �� ��������
        {
            if (_infoItem)
            {
                Destroy(_infoItem);
            }
        }

        private void GetInfoItem(int IDItem)//��������� ����������  � �������� � �������� ���������� ����� ���������� ��������������� ����
        {
           
            ItemData itemData = ItemDatabase.LookIDItem(IDItem);
            string _itemDamageinfo;
            switch (itemData.GetTypeEquipment) // ����� ������� � ����������� �� ���� ������ ���� �������
            {
                case TypeEquipment.weapon:
                    _itemDamageinfo = "����: " + itemData.GetMinDamageWeapon + " - " + itemData.GetMaxDamageWeapon;
                    break;
                default:
                    _itemDamageinfo = "�����: " + itemData.GetArmorItem;
                    break;

            }

           
            _infoItem.GetComponent<InfoItem>().SetStringInfo(itemData.GetDiscription, _itemDamageinfo,abilityDiscription[itemData.GetSpecialAttack]);//�������� ����� ���������� �������� ���������� ��� �����������

        }


        #endregion

      
      

        
        public bool returnType(TypeEquipment type,int slot)//�������� �� ��� ����� � ����������
        {
            if (type==myEquipment[slot].GetComponent<InventoryDrop>().typeEquipment) return true;
            else return false;


        } 

      

        public void instantiateEquipmentSlot()//��������� ������ ���������� � myEquipment
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
            BoltLog.Warn("������� ����� ���������");
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
            ItemData ItemData = ItemDatabase.LookIDItem(ID);
            if (ItemData != null)
            {
                if (ItemData.GetIconItem != null)
                    newItem.GetComponent<Image>().sprite = ItemData.GetIconItem;
            }

            if (quantity > 1)
                newItem.transform.GetChild(0).GetComponent<Text>().text = quantity.ToString();
            newItem.GetComponent<InventoryDrag>().slot = slotID;
            newItem.GetComponent<InventoryDrag>().IDItem = ItemData.GetIDItem;
            newItem.transform.SetParent(ListObject[slotID].transform);
            newItem.GetComponent<RectTransform>().localPosition = Vector3.zero;
            newItem.GetComponent<RectTransform>().localScale = Vector3.one;


        }
        


        public void refreshItem(List<GameObject> gameObjects, NetworkArray_Objects<item> myState)
        {
            GameObject PAL = GameObject.FindGameObjectWithTag("Player");

            if (!PAL.GetComponent<BoltEntity>().IsOwner) return;
            
               
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
