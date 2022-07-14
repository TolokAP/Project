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

        public override void OnEvent(swapSlots evnt)// Изменение слота в инвентаре
        {
            if (BoltNetwork.IsClient)
            {
                Player.GetComponent<InventoryPlayerController>().swapSlots(evnt);
                BoltLog.Warn("ПОлучено событие в ClientData смена слота");
            }
        }

        public override void OnEvent(destroyItem evnt)
        {

            if (BoltNetwork.IsClient)
            {
                Player.GetComponent<InventoryPlayerController>().deleteItem(evnt);
                BoltLog.Warn("Получено событие в ClientData удаление предмета");
            }

        }



       


        public override void StreamDataStarted(BoltConnection connection, UdpChannelName channel, ulong streamID)
        {
            BoltLog.Warn("Начало передачи данных от  {0} channel {1} ", connection, channel);
        }

        public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)//Получение данных игрока от сервера
        {
            BoltLog.Warn("Канал доставки" + data.Channel.ToString());
            if (data.Channel.Equals(SendDataChannel))//Загрузка данных 
            {
                BoltLog.Warn("Работает получение данных");
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

                BoltLog.Warn("Идентификатор" + connection.ToString());

            }
            else
            {
                BoltLog.Warn("AcceptedToken is NULL");
            }

            Debug.Log("Подключение" + connection.ToString());

        }


        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            if (scene == "GameScene")
            {
                BoltLog.Warn("Загружена сцена");

                //PlayerConnecting.Post(_name);//Ручная авторизация
                PlayerConnecting.Post("1");//Автоматическая авторизация
            }


        }




    }
}