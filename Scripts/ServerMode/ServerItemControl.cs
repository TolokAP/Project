
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;


namespace Player
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server, "GameScene")]
    public class ServerItemControl : GlobalEventListener
    {
        public ItemDB itemDB;
      
        public ItemData lookUpID(int ID)// Получение предмета из базы предметов.
        {
           
            foreach (ItemData ItemData in itemDB.itemDatabase)
            {
                if (ItemData.ID == ID)
                {
                    return ItemData;
                }
            }
            return null;

        }

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            itemDB = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDB>();
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
            BoltNetwork.Instantiate(lookUpID(evnt.ID).prefab, NetIDToken);


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