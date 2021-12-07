
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;


namespace Player
{
    public enum SwapType { toEquipment, fromEquipment, swapSlotInventory };
    public class InventoryPlayerController : EntityEventListener<IPlayer>
    {
        public InventoryPlayer myInventory;
        public BoltEntity boltEntity;

       
        private SwapType _swapType;


        public override void OnEvent(PickUpItem evnt)
        {
          
            if (evnt.Entity.IsOwner)
            {
                BoltLog.Warn("Работает получения события от сервера к клиенту Получает только владелец");
                //state.PickUP();
            
                addItem(evnt.ID, 1);
            }
           
        }
        public void swapSlots(swapSlots evnt)
        {
            //step1: check if match
            //step2: check is stackable
            //step3: check if "to" is full, if full then just swap
            //check if they add up to beyond stack cap
            //if they not beyond cap, add to "to" and make "from" empty
            //otherwise add to "to" until full and subtract from "from"
            _swapType = (SwapType)evnt.swapType;
            if (_swapType == SwapType.fromEquipment)
            {
                BoltLog.Warn("Из Экипировки");
                ChangeItemInventory(state.items, state.Equipmnet, evnt.to, evnt.from);

            }
            else if (_swapType == SwapType.toEquipment)
            {
                BoltLog.Warn("В Экипировку");
                if (!myInventory.returnType(myInventory.lookUpID(state.items[evnt.from].ID).type, evnt.to)) return;
                else { ChangeItemInventory(state.Equipmnet, state.items, evnt.to, evnt.from); }
            }
            else
            {
                ChangeItemInventory(state.items, state.items, evnt.to, evnt.from);

            }


        }

        public void deleteItem(destroyItem evnt)
        {
            
            var iiw = instatiateItemInWorld.Create(GlobalTargets.OnlyServer);// Отправялем на сервер событие, предмет из инвертаря попадает в открытый мир.
            iiw.ID = state.items[evnt.slot].ID;
            iiw.NetworkID = boltEntity.NetworkId;
            iiw.Send();

            state.items[evnt.slot].ID = 0;
            state.items[evnt.slot].quantity = 0;

        }

      
        public void ChangeItemInventory(NetworkArray_Objects<item> myStateTo, NetworkArray_Objects<item> myStateFrom, int to, int from)
        {

            if (myStateTo[to].ID == myStateFrom[from].ID)
            {
                if (myInventory.lookUpID(myStateTo[to].ID).stackable)
                {
                    if (myInventory.lookUpID(myStateTo[to].ID).stackSize > myStateTo[to].quantity)
                    {
                        int moveQuanity = 0;
                        //how many item units are moving

                        int moveSpace = myInventory.lookUpID(myStateTo[to].ID).stackSize - myStateTo[to].quantity;
                        //how much room is available in "to"

                        int fromQuantity = myStateFrom[from].quantity;

                        if (moveSpace >= fromQuantity)
                        {
                            moveQuanity = fromQuantity;
                        }
                        else
                        {
                            moveQuanity = moveSpace;
                        }

                        myStateTo[to].quantity += moveQuanity;
                        myStateFrom[from].quantity -= moveQuanity;

                        if (myStateFrom[from].quantity == 0)
                            myStateFrom[from].ID = 0;

                        return;
                    }
                }
            }

            int oldItem = myStateTo[to].ID;
            myStateTo[to].ID = myStateFrom[from].ID;
            myStateFrom[from].ID = oldItem;

            int oldQuantity = myStateTo[to].quantity;
            myStateTo[to].quantity = myStateFrom[from].quantity;
            myStateFrom[from].quantity = oldQuantity;

        }

        public void OnStatsChanged(IState istate, string propertPath, ArrayIndices arrayIndices)
        {
            var index = arrayIndices[0];
            var localstate = (IPlayer)state;
            var equipment = localstate.Equipmnet[index];
            //BoltLog.Warn("Изменено {0} индекс {1}  измененый массив {2}", propertPath, index, equipment);
            if (myInventory.mySlots.Count != 0)
            {
                myInventory.refreshItem(myInventory.mySlots, state.items);
                ChangeInventoryItemPlayerToServer.Post(index, state.items[index].ID, state.items[index].quantity, state.Login, true);
            }
           

        }

        public void EquipmentSwapSlot(IState istate, string propertPath, ArrayIndices arrayIndices)
        {
           

            var index = arrayIndices[0];
            var localstate = (IPlayer)state;
            var equipment = localstate.Equipmnet[index];
            //BoltLog.Warn("Изменено {0} индекс {1}  измененый массив {2}", propertPath, index, equipment);
            if (myInventory.myEquipment.Count != 0)
            {
                myInventory.refreshItem(myInventory.myEquipment, state.Equipmnet);
                ChangeInventoryItemPlayerToServer.Post(index, state.Equipmnet[index].ID, state.Equipmnet[index].quantity, state.Login,false);
            
            
            
            }
            
          

        }

        public override void Attached()
        {
            if (BoltNetwork.IsClient)
            {
                if (entity.IsOwner)
                {
                    boltEntity = GetComponent<BoltEntity>();
                    myInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPlayer>();
                    myInventory.PlayerController = this;
                   
                    state.AddCallback("slots", () => { myInventory.instantiateSlots(state.slots); });
                    state.AddCallback("items[]", OnStatsChanged);
                    state.AddCallback("Equipmnet[]", EquipmentSwapSlot);


                   
                  
                }
            }

        }

       
        public void addItem(int ID, int quantity)
        {
            //if stackable try adding to non full stacks
            ItemData ItemData = myInventory.lookUpID(ID);
            if (ItemData.stackable)
            {
                int currentQuantity = quantity;


                for (int i = 0; i < state.slots; i++)
                {
                    if (state.items[i].ID == ID)
                    {
                        if (state.items[i].quantity < ItemData.stackSize)
                        {
                            int moveQuanity;

                            int moveSpace = ItemData.stackSize - state.items[i].quantity;

                            if (moveSpace >= quantity)
                            {
                                moveQuanity = quantity;
                            }
                            else
                            {
                                moveQuanity = moveSpace;
                            }

                            state.items[i].quantity += moveQuanity;
                            currentQuantity -= moveQuanity;
                            if (currentQuantity == 0)
                            {
                                return;
                            }

                        }
                    }

                }
            }




            for (int i = 0; i < state.slots; i++)
            {
                if (state.items[i].ID == 0)
                {
                    state.items[i].ID = ID;
                    state.items[i].quantity = quantity;
                    return;
                }

            }


            Debug.Log("no space");
        }

        // Update is called once per frame
        void Update()
        {
            if (BoltNetwork.IsClient)
            {
                if (entity.IsOwner)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        addItem(1, 1);
                    }
                    if (Input.GetKeyDown(KeyCode.R))
                        addItem(2, 1);
                    if (Input.GetKeyDown(KeyCode.O))
                        addItem(5, 1);
                    if (Input.GetKeyDown(KeyCode.P))
                        addItem(7, 1);

                    if (Input.GetKeyDown(KeyCode.Q))
                        state.slots--;
                    if (Input.GetKeyDown(KeyCode.W))
                        state.slots++;
                   
                }

            }

        }
    }
}