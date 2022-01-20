
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;


namespace Player
{
  
    public class ServerItemControl : GlobalEventListener
    {
        public ItemDatabase itemDatabase;
      
       

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            
            
        }
        

        public override void OnEvent(instatiateItemInWorld evnt)//������� ���������� �������, ������� ����� ������� � ��������� ���.
        {
            BoltLog.Warn("�������� ��������");
            var NetIDToken = new NetworkIDToken
            {
                Hand = (int)WeaponHandPosition.None,
                NetworkID = evnt.NetworkID.PackedValue,
                ID = evnt.ID
            };
            BoltNetwork.Instantiate(itemDatabase.LookIDItem(evnt.ID).GetPrefabItem, NetIDToken);


        }

        public override void OnEvent(PickUpItem evnt)//��������� ������� �������� �������� (�������� ��������: ������). �������� ������� ������ ������� ������ �������.
        {
            GameObject gameObject = BoltNetwork.FindEntity(evnt.NetworkID);
            BoltNetwork.Destroy(gameObject);
            var ert = PickUpItem.Create(evnt.Entity, EntityTargets.Everyone);
            ert.Entity = evnt.Entity;
            ert.ID = evnt.ID;
            ert.Send();
            BoltLog.Warn("������� �� ������� �������");

        }




    }
}