using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine;
using UdpKit;
using System.Text;

using Newtonsoft.Json;


namespace Player
{
    [BoltGlobalBehaviour(BoltNetworkModes.Client)]
    public class ClientData : GlobalEventListener
    {
        public GameObject Player;
        private string _name;
        public BoltEntity ThisEntity;

        private const string SendData = "SendDataPlayer";
        private const string SendSkillData = "SendDataSkillPlayer";

        private static UdpChannelName SendDataChannel;
        private static UdpChannelName SendDataSkillChannel;

     
        private PLayer _player;









        private void Start()
        {
            _player = new PLayer();
        }
        public override void BoltStartBegin()
        {
            BoltNetwork.RegisterTokenClass<ProtocolTokenLogin>();
            BoltNetwork.RegisterTokenClass<AuthResultToken>();
            SendDataChannel = BoltNetwork.CreateStreamChannel(SendData, UdpChannelMode.Reliable, 1);
            SendDataSkillChannel = BoltNetwork.CreateStreamChannel(SendSkillData, UdpChannelMode.Reliable, 1);
        }

        public override void OnEvent(swapSlots evnt)// ��������� ����� � ���������
        {
            if (BoltNetwork.IsClient)
            {
                Player.GetComponent<InventoryPlayerController>().swapSlots(evnt);
                BoltLog.Warn("�������� ������� � ClientData ����� �����");
            }
        }

        public override void OnEvent(destroyItem evnt)
        {

            if (BoltNetwork.IsClient)
            {
                Player.GetComponent<InventoryPlayerController>().deleteItem(evnt);
                BoltLog.Warn("�������� ������� � ClientData �������� ��������");
            }

        }



        public override void EntityAttached(BoltEntity entity)// ��������� �������� ������.
        {

            ThisEntity = entity;
        }


        public override void StreamDataStarted(BoltConnection connection, UdpChannelName channel, ulong streamID)
        {
            BoltLog.Warn("������ �������� ������ ��  {0} channel {1} ", connection, channel);
        }

        public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)//��������� ������ ������ �� �������
        {
            BoltLog.Warn("����� ��������" + data.Channel.ToString());
            if (data.Channel.Equals(SendDataChannel))//�������� ������ 
            {
                BoltLog.Warn("�������� ��������� ������");
                string dataP = Encoding.UTF8.GetString(data.Data);
                BoltLog.Warn("�������� �� {0} ������ {1} ", connection, dataP);
                _player = JsonConvert.DeserializeObject<PLayer>(dataP);
                

                CreatePlayer();
            }

          
        }

      

       
        private void SetPlayerState()
        {
            ThisEntity.GetState<IPlayer>().Name = _player.Name;
            ThisEntity.GetState<IPlayer>().TotalHealth = _player.Health;
            ThisEntity.GetState<IPlayer>().Login = _player.Login;
            BoltLog.Warn("������ �������" + _player.combatSkills.Length + " �������" + ThisEntity.GetState<IPlayer>().Skills.Length);
            for(int i = 0; i < _player.combatSkills.Length; i++)
            {
                ThisEntity.GetState<IPlayer>().Skills[i] = _player.combatSkills[i];
            }

            ThisEntity.GetState<IPlayer>().slots = 5;
            
            BoltLog.Warn("������ �������" + _player.inventory.GetLength(0));
            for (int i = 0; i <_player.inventory.GetLength(0); i++)
            {
                ThisEntity.GetState<IPlayer>().items[i].ID = _player.inventory[i, 0];
                ThisEntity.GetState<IPlayer>().items[i].quantity = _player.inventory[i, 1];
            }
            for(int i=0; i < _player.equipment.Length; i++)
            {
                ThisEntity.GetState<IPlayer>().Equipmnet[i].ID = _player.equipment[i];

            }
            for(int i = 0; i < _player.stats.Length; i++)
            {
                ThisEntity.GetState<IPlayer>().Stats[i] = _player.stats[i];
            }
          

        }


        private void CreatePlayer()
        {
            var spawnPosi = new Vector3(Random.Range(20, 74), 30, 36);
            Player = BoltNetwork.Instantiate(BoltPrefabs.WarriorPrefab, spawnPosi, Quaternion.identity);
            SetPlayerState();


        }

        public override void Connected(BoltConnection connection)
        {

            AuthResultToken acceptToken = connection.AcceptToken as AuthResultToken;
            if (acceptToken != null)
            {
                _name = acceptToken.Id;

                BoltLog.Warn("�������������" + connection.ToString());

            }
            else
            {
                BoltLog.Warn("AcceptedToken is NULL");
            }

            Debug.Log("�����������" + connection.ToString());

        }


        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            if (scene == "GameScene")
            {
                BoltLog.Warn("��������� �����");

                //PlayerConnecting.Post(_name);//������ �����������
                PlayerConnecting.Post("1");//�������������� �����������
            }


        }




    }
}