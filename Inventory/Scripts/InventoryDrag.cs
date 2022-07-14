using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

using Photon.Bolt;
using Photon.Bolt.Utils;

namespace Player
{
    public class InventoryDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
    {
        
        public GameObject inventory;
        public GameObject itemSlot;
        public int slot = -1;
        public Transform startParent;
        public GameObject dragPosition;
        public int IDItem = -1;

        public GameObject infoItemPrefab;
        public GameObject _infoItem;
       

      
      

        public void OnPointerClick(PointerEventData eventData)
        {
            BoltLog.Error("Есть скрипт или нет" + eventData.pointerClick.name + eventData.pointerClick.TryGetComponent<InventoryDrag>(out InventoryDrag inventoryDrag));
           
            if (eventData.pointerClick.TryGetComponent<InventoryDrag>(out InventoryDrag inventoryDrag1))
            {
                BoltLog.Error(slot);
                inventory.GetComponent<InventoryPlayer>().OpenWindowItemInfo(gameObject.transform, IDItem);
            }
            

        }



        private void Start()
        {
            startParent = transform.parent;
            inventory = GameObject.FindGameObjectWithTag("Inventory");
            dragPosition = GameObject.FindGameObjectWithTag("Canvas");
           
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
           
            
           


            if (inventory == null) return;
            GetComponent<Image>().raycastTarget = false;
           
            BoltLog.Warn("Метод ON BEgin Drag");

            if (inventory.GetComponent<InventoryPlayer>().selectedSlot == null)
                inventory.GetComponent<InventoryPlayer>().selectedSlot = transform.parent.gameObject;
            else if (inventory.GetComponent<InventoryPlayer>().selectedSlot == transform.parent.gameObject)
                inventory.GetComponent<InventoryPlayer>().selectedSlot = null;
            transform.SetParent(dragPosition.transform, true);
            inventory.GetComponent<InventoryPlayer>().CloseWindowItemInfo();
        }

        public void OnDrag(PointerEventData eventData)
        {
          
            transform.position = Input.mousePosition;
      
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponent<Image>().raycastTarget = true;
            transform.SetParent(startParent.transform, true);
            inventory.GetComponent<InventoryPlayer>().selectedSlot = null;
            
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<InventoryDrop>() == null)
                {
                    transform.localPosition = Vector3.zero;
                    if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Image>() == null)
                    {
                        if (startParent.GetComponent<InventoryDrop>().slotType == InventoryDrop.SlotType.inventory)
                        {
                            BoltLog.Warn("Удаление предмета");
                            var evnt = destroyItem.Create(GlobalTargets.OnlySelf);
                            evnt.slot = slot;
                            evnt.Send();
                        }

                    }


                }
                if(eventData.pointerCurrentRaycast.gameObject.transform == startParent.transform)
                {
                    transform.localPosition = Vector3.zero;
                }
              

        }
       
        public void OnPointerDown(PointerEventData eventData)
        {
          
          
        }
    }
}