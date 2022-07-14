using Photon.Bolt;
using Photon.Bolt.Utils;
using UnityEngine;
using UdpKit;
using System.Text;



namespace Player
{
    [BoltGlobalBehaviour(BoltNetworkModes.Client)]
    public class ClientData : GlobalEventListener
    {
        public GameObject Player;
        private string _name;
  

        private const string SendData = "SendDataPlayer";
        private const string SendSkillData = "SendDataSkillPlayer";

        private static UdpChannelName SendDataChannel;
        private static UdpChannelName SendDataSkillChannel;

     
      









      
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
       
                CreatePlayer(dataP);
            }

          
        }

      

       
       

        private void CreatePlayer(string data)
        {
            var dataPlayer = new LoadStatePlayer();
            dataPlayer.data = data;
            var spawnPosition = new Vector3(Random.Range(20, 74), 30, 36);
            Player = BoltNetwork.Instantiate(BoltPrefabs.WarriorPrefab, dataPlayer, spawnPosition, Quaternion.identity);
         


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