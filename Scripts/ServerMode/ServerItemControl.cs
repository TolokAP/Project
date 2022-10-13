
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Player
{

    public class ServerItemControl : GlobalEventListener
    {
        public ItemDatabase itemDatabase;
        public List<int> idItem = new List<int>();

        public GameObject LootBox;

       


        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            
            
        }


        public override void OnEvent(LootItemInPlayerToServer evnt)
        {
            idItem.Clear();
            idItem = JsonConvert.DeserializeObject<List<int>>(evnt.EquipmentID);
            foreach( int value in idItem)
            {
                BoltLog.Error("Номер экипировки" + value);
            
            }
           

            var token = new LootEquipID();
            token.EquipID = evnt.EquipmentID;
           
           
            BoltNetwork.Instantiate(LootBox, token,evnt.TransformDeadPlayer,Quaternion.identity);
            
        }

        public override void OnEvent(IDLootItem evnt)// событие которое получает сервер когда предмет забирают с сущности трупа, отправляется из ItemDrag.
        {
            BoltLog.Error("Получено сообщение о луте предмета " + evnt.ID + " Сущность " + evnt.EntityPlayer);
            
            int size = evnt.EntityDeadPlayer.GetState<IDeadPlayerState>().IdItem.Length;// Получение размера массива

           
            for (int i = 0; i < size; i++) //Проверка на присутствие предмета для возможност лута.
            {
                if(evnt.EntityDeadPlayer.GetState<IDeadPlayerState>().IdItem[i] == evnt.ID)
                {
                    BoltLog.Error("Такое Айди есть");
                    evnt.EntityDeadPlayer.GetState<IDeadPlayerState>().IdItem[i] = 0;
                    var evt = PickUpItem.Create(evnt.EntityPlayer);
                    evt.Entity = evnt.EntityPlayer;
                    evt.ID = evnt.ID;
                    evt.Send();

                }
                
            }
           
            
        }
        public override void OnEvent(instatiateItemInWorld evnt)//Событие размещение обьекта, который игрок выкинул в открыйтый мир.
        {
            BoltLog.Warn("Создание предмета");
            var NetIDToken = new NetworkIDToken
            {
                Hand = (int)WeaponHandPosition.None,
                NetworkID = evnt.NetworkID.PackedValue,
                ID = evnt.ID
            };
            BoltNetwork.Instantiate(itemDatabase.LookIDItem(evnt.ID).GetPrefabItem, NetIDToken);


        }

        public override void OnEvent(PickUpItem evnt)//Получения события поднятие предмета (Владелец предмета: Сервер). Отправка события игроку который поднял предмет.
        {
            GameObject gameObject = BoltNetwork.FindEntity(evnt.NetworkID);
            BoltNetwork.Destroy(gameObject);
            var ert = PickUpItem.Create(evnt.Entity, EntityTargets.Everyone);
            ert.Entity = evnt.Entity;
            ert.ID = evnt.ID;
            ert.Send();
            BoltLog.Warn("Получил на сервере событие");

        }




    }
}