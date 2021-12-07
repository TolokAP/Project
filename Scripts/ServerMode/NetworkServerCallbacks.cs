
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

using UdpKit;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
namespace Player
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class NetworkServerCallbacks : GlobalEventListener
    {
        private Dictionary<string, string> PlayerInfo;//Словарь, хранит регистрационные данные игроков

        public Dictionary<BoltConnection, string> PlayerOnline;

       

        public PLayer player;

        private const string SendData = "SendDataPlayer";
        private const string SendSkillData = "SendDataSkillPlayer";

        private string dataUser = "Data/user.txt";

        private static UdpChannelName SendDataChannel;
        private static UdpChannelName SendDataSkillChannel;


        #region Unity
        private void Start()
        {
            PlayerInfo = new Dictionary<string, string>();
            PlayerOnline = new Dictionary<BoltConnection, string>();
            player = new PLayer();

            if (File.Exists(dataUser))
            {
                LoadRegDataPlayer(dataUser);

            }
            else
            {
                string dir = "Data";
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                if (!directoryInfo.Exists) directoryInfo.Create();
                StreamWriter sw = new StreamWriter(dataUser);
                sw.WriteLine("{}");
                sw.Close();
                Debug.Log("Файл создан");
            }
        }

        #endregion

        #region BoltCallBacks

        public override void BoltStartBegin()
        {
            BoltNetwork.RegisterTokenClass<ProtocolTokenLogin>();
            BoltNetwork.RegisterTokenClass<AuthResultToken>();
            SendDataChannel = BoltNetwork.CreateStreamChannel(SendData, UdpChannelMode.Reliable, 1);
            SendDataSkillChannel = BoltNetwork.CreateStreamChannel(SendSkillData, UdpChannelMode.Reliable, 1);
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)//Ручная или автоматическая авторизация игрока
        {
            BoltLog.Warn("Connect Request" + "Работает");
            LoadRegDataPlayer(dataUser);

            var userToken = token as ProtocolTokenLogin;

            if (userToken != null)
            {
                if (userToken.Create)
                {
                    if (!PlayerInfo.ContainsKey(userToken.Username))
                    {
                        NewPlayerInServer(userToken.Username, userToken.Password, userToken.NamePlayer, userToken.Class);

                        AuthResultToken resultToken = new AuthResultToken(userToken.Username);
                        BoltNetwork.Accept(endpoint, resultToken);
                        return;
                    }
                    else
                    {
                        Debug.Log("Логин уже есть");
                        AuthResultToken resultToken = new AuthResultToken { create = false };
                        BoltNetwork.Refuse(endpoint, resultToken);
                    }

                }
                else
                {
                    if (AuthUser(userToken.Username, userToken.Password))
                    {

                        AuthResultToken resultToken = new AuthResultToken(userToken.Username);
                        BoltNetwork.Accept(endpoint, resultToken);
                        return;
                    }
                }
            }
            BoltNetwork.Refuse(endpoint, AuthResultToken.Invalid);
        }
        public override void Disconnected(BoltConnection connection) //Убирает игрока из списка в случае отключения
        {

            PlayerOnline.Remove(connection);

        }


        #endregion
        private bool AuthUser(string user, string pass)
        {

            if (PlayerInfo.ContainsKey(user) && !PlayerOnline.ContainsValue(user))
            {
                PlayerInfo.TryGetValue(user, out string a);
                if (a == pass)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }


        }

       
        private void LoadRegDataPlayer(string path)//Загрузка регистрационных данных игроков
        {

            PlayerInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>
                                     (File.ReadAllText(path));

        }



        private void NewPlayerInServer(string login, string pass, string name, string _class)//Добавление нового игрока в файл user.txt
        {
            PlayerInfo.Add(login, pass);
            string json = JsonConvert.SerializeObject(PlayerInfo);
            File.WriteAllText(dataUser, json);
            CreatePlayerData(login, name);

        }

     
        private PLayer CreatePlayer(string login, string name)//Создание игрока
        {
            player.Login = login;
            player.Name = name;
            player.Health = 10;
            player.Level = 0;
            player.Mana = 10;
            for(int i = 0; i < player.combatSkills.Length; i++)
            {
                player.combatSkills[i] = 30.0f;
            }
            player.SetSkill(0, 50.0f);
            for(int i = 0; i < player.stats.Length; i++)
            {
                player.stats[i] = 10;
            }
            player.inventory[0, 0] = 2;
            player.inventory[0, 1] = 1;
          
            return player;

        }

      
        private string GetPathDataPlayer(string login)//Получение пути сохранения файлов
        {
            string v = "Data/" + login + "/" + login + ".txt";
                                 
            return v;
        }



        private void CreatePlayerData(string login, string name)//Создание файла игрока с его характеристиками
        {
            string dir = "Data/" + login + "/";
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            if (!directoryInfo.Exists) directoryInfo.Create(); //Создание папки для сохранения файлов игрока


            string json = JsonConvert.SerializeObject(CreatePlayer(login, name));//Создание  характеристики игрока
            File.WriteAllText(GetPathDataPlayer(login), json);
           
          

        }
       
        private void SaveItemsPlayerinServer(string login, PLayer pLayer)//Записывает данные игрока в файл на сервере
        {
            string json = JsonConvert.SerializeObject(pLayer);
            File.WriteAllText(GetPathDataPlayer(login), json);
            BoltLog.Warn("Получено событие изменение инвентаря");
            
        }
      




        

        #region BoltEvent

        public override void OnEvent(PlayerConnecting evnt)//Событие отправки данных игрока подключившемуся игроку
        {

            BoltConnection connection = evnt.RaisedBy;
            BoltLog.Warn("Отправляем данные");
            PlayerOnline.Add(connection, evnt.LoginPlayer);
            foreach (var vlue in PlayerOnline)
            {
                BoltLog.Warn(vlue.ToString());
            }

            PLayer _player = JsonConvert.DeserializeObject<PLayer>
                                      (File.ReadAllText(GetPathDataPlayer(evnt.LoginPlayer)));

            string dataPlayer = JsonConvert.SerializeObject(_player);
            byte[] data = Encoding.UTF8.GetBytes(dataPlayer);
            connection.StreamBytes(SendDataChannel, data);

        }
        public override void OnEvent(ChangeInventoryItemPlayerToServer evnt)//Событие изменение инвентаря и экипировки
        {
            PLayer _player = JsonConvert.DeserializeObject<PLayer>
                                    (File.ReadAllText(GetPathDataPlayer(evnt.Login)));
            if (evnt.Inventory)
            {
                _player.inventory[evnt.Slot, 0] = evnt.ID;
                _player.inventory[evnt.Slot, 1] = evnt.Quantity;

            }
            else
            {
                _player.equipment[evnt.Slot] = evnt.ID;

            }
            SaveItemsPlayerinServer(evnt.Login, _player);
        }
        public override void OnEvent(ChangeSkillAndStats evnt)
        {
            PLayer _player = JsonConvert.DeserializeObject<PLayer>
                                               (File.ReadAllText(GetPathDataPlayer(evnt.Login)));

          
          
            if (evnt.Skill)
            {
                float value = _player.combatSkills[evnt.SkillStats];
                _player.combatSkills[evnt.SkillStats] = (float)System.Math.Round(value + 0.1f, 1);


                BoltLog.Warn("Значение после " + _player.combatSkills[evnt.SkillStats].ToString());

            }
            string json = JsonConvert.SerializeObject(_player);
            File.WriteAllText(GetPathDataPlayer(evnt.Login), json);

        }

        #endregion
    }
}












