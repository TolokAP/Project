
using UnityEngine;
using Photon.Bolt;
using Newtonsoft.Json;
using Photon.Bolt.Utils;
using System.Collections.Generic;
using System.Collections;

namespace Player {
    public class PlayerState : EntityEventListener<IPlayer>
    {
        private BoltEntity _boltEntity;
        private UIPlayer _UIPlayer;
        [SerializeField]
        private GameObject _characterCamera;

        [SerializeField]
        private ItemDatabase _itemDatabase;
        public int localHealth;
        public int _maxHealth;

        private PLayer _player;


        [SerializeField]
        private GameObject _DeathWindow;

        [SerializeField]
        private Transform _RevivePortal;
        private CharacterController ch_controller;
        public GameObject body;

        public List<int> idItem = new List<int>();

        public GameObject DeadPlayerWindow;

        [SerializeField]
        private GameObject _DeadPlayer;

        public override void Attached()
        {
            DeadPlayerWindow = GameObject.FindGameObjectWithTag("DeadPlayerWindow");

            if (!entity.IsOwner) {

                ChangeLayer("OtherPlayer");// Если другой игрок то установить другой слой и отключить коллайдер
                
                return;
            } 
            
            _player = new PLayer();
            var dataToken = (LoadStatePlayer)entity.AttachToken;
            _player = JsonConvert.DeserializeObject<PLayer>(dataToken.data);
            _UIPlayer = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIPlayer>();
            _RevivePortal = GameObject.FindGameObjectWithTag("RevivePortal").transform;
            ch_controller = GetComponent<CharacterController>();
            Instantiate(_characterCamera, gameObject.transform);
            _boltEntity = GetComponent<BoltEntity>();
            state.AddCallback("CurrentHealth", CurrentHealthCallBack);//Обратный вызов Текущее здоровье
            state.AddCallback("TotalHealth", MaxHealthCallBack);//Обратный вызов максимальное здоровье
            state.AddCallback("Skills[]", SkillPlayerCallbacks);//Обратный вызов изменения скилла
            state.AddCallback("Stats[]", StatsPlayerCallbacks); //Обратный вызов изменения характеристик
            state.AddCallback("SpecialAttack[]", UpdateSpecialAttack);//Обратный вызов изменение специальных аттак
            state.AddCallback("Armor", UpdateArmorUI); // Обратный вызов Armor
            state.AddCallback("Damage[]", UpdateDamageUI);//Обратный вызов обновление Damage
           
            SetPlayerState();


            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();//Добавляем коллайдер 
            sphereCollider.radius = 10;
            sphereCollider.isTrigger = true;

           
        }

        private void ChangeLayer(string value)//Смена слоя у игроков и отключение коллайдеров
        {

            
            Transform[] layers = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform layer in layers)
            {
                layer.gameObject.layer = LayerMask.NameToLayer(value);

            }
            
        }

       

        private void UpdateDamageUI(IState state1, string propertyPath, ArrayIndices arrayIndices)
        {
            int index = arrayIndices[0];
           
            _UIPlayer.UpdateDamageUI(state.Damage[0].ToString() + "-" + state.Damage[1].ToString());

        }

        private void UpdateArmorUI(IState state, string propertyPath, ArrayIndices arrayIndices)
        {
            _UIPlayer.UpdateArmorText(_boltEntity.GetState<IPlayer>().Armor.ToString());
        }

        private void SetPlayerState() // Переносит данные из файла в сущность Bolt.
        {
            //_boltEntity.GetState<IPlayer>().Name = _player.Name;

            _boltEntity.GetState<IPlayer>().Name = Random.Range(0, 100).ToString();

            _boltEntity.GetState<IPlayer>().TotalHealth = _player.Health;
            _boltEntity.GetState<IPlayer>().Login = _player.Login;
          

            for (int i = 0; i < _player.combatSkills.Length; i++)
            {
                _boltEntity.GetState<IPlayer>().Skills[i] = _player.combatSkills[i];
            }

            _boltEntity.GetState<IPlayer>().slots = 5;

           
            for (int i = 0; i < _player.inventory.GetLength(0); i++)
            {
                _boltEntity.GetState<IPlayer>().items[i].ID = _player.inventory[i, 0];
                _boltEntity.GetState<IPlayer>().items[i].quantity = _player.inventory[i, 1];
            }
            for (int i = 0; i < _player.equipment.Length; i++)
            {
                _boltEntity.GetState<IPlayer>().Equipmnet[i].ID = _player.equipment[i];


            }
            for (int i = 0; i < _player.stats.Length; i++)
            {
                _boltEntity.GetState<IPlayer>().Stats[i] = _player.stats[i];
            }
          

        }



        private void UpdateSpecialAttack(IState state2, string propertyPath, ArrayIndices arrayIndices)
        {
            int index = arrayIndices[0];
            if (state.SpecialAttack[index].AttackNumber == 0) { _UIPlayer.UpdateIconSpecialAttack(index, state.SpecialAttack[index].AttackNumber, false); }
            else { _UIPlayer.UpdateIconSpecialAttack(index, state.SpecialAttack[index].AttackNumber, true); }

        }


        private void StatsPlayerCallbacks(IState state, string propertyPath, ArrayIndices arrayIndices)
        {
            var index = arrayIndices[0];
            _UIPlayer.UpdateUI(index, _boltEntity.GetState<IPlayer>().Stats[index]);

        }




        #region Bolt Callbacks
        private void MaxHealthCallBack()//CallBack изменение максимального здоровья.
        {
            if (!entity.IsOwner) return;
            

            if (state.TotalHealth != state.CurrentHealth)
            {
                int value = state.TotalHealth - _maxHealth;
                state.CurrentHealth += value;
                _maxHealth = state.TotalHealth;
                _UIPlayer.UpdateHealthUI(state.CurrentHealth, state.TotalHealth);
                 
            }

            

        }
        private void CurrentHealthCallBack()//CallBack изменение текущего здоровья.
        {

            if (!entity.IsOwner) return;

           
            if (state.CurrentHealth <= 0) // Текущеее здоровье меньше нуля
            {
                if (state.Dead == false)
                {

                    state.CurrentHealth = 0;
                    state.DeathTrigger();
                    state.Dead = true;
                    _ = DeadPlayer.Post(entity, true);


                    for (int i = 0; i < state.Equipmnet.Length; i++)
                    {
                        if (state.Equipmnet[i].ID != 0)
                        {

                            idItem.Add(state.Equipmnet[i].ID);
                            state.Equipmnet[i].ID = 0;
                            state.Equipmnet[i].quantity = 0;
                        }
                    }
                
                 
                  
                }

            }
            
            _UIPlayer.UpdateHealthUI(state.CurrentHealth, state.TotalHealth);
             
            
          
        }

        public void AnimatiomEventDeadPlayerEnd()//Событие срабатывает в конце анимации смерти
        {
           
            _DeadPlayer =  Instantiate(body, transform.position, transform.rotation, DeadPlayerWindow.transform); // создание трупа для лута

            ChangeLayer("Invisible");// основного персонажа делает невидимым


            if (entity.IsOwner)
            {

                _DeathWindow.SetActive(true); //Открывает окно "смерти персонажа"

                string equip = JsonConvert.SerializeObject(idItem);

                _ = LootItemInPlayerToServer.Post(GlobalTargets.OnlyServer, equip, _DeadPlayer.transform.position); // отправка события на сервер 

                ch_controller.enabled = false;

                transform.position = _RevivePortal.position;//Телепорт к месту воскрешения


                ch_controller.enabled = true;

                ChangeLayer("Player");
            }
            else
            {
                ChangeLayer("OtherPlayer");
            }
        }


      

        public void PlayEffect(GameObject Effect)
        {
            BoltLog.Error("Работает метод эффекта");
            GameObject _effect =  Instantiate(Effect,gameObject.transform);
            Destroy(_effect, 5f);

        }


        public void OffDeathWindow()// Метод на кнопке окна "Смерти Персонажа"
        {
            _DeathWindow.SetActive(false);//Закрываем окно

            
            state.Dead = false;
        
        }

        private void SkillPlayerCallbacks(IState state, string propertyPath, ArrayIndices arrayIndices)
        {
            var index = arrayIndices[0];
            _UIPlayer.UpdateUI(index, _boltEntity.GetState<IPlayer>().Skills[index]);


        }


        #endregion





    }
}