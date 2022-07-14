
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine.UI;
using TMPro;


namespace Player
{
    public class Health : EntityEventListener<IPlayer>
    {
        public int localHealth;
        public int _maxHealth;

        public BoltEntity GO;

        public Slider healthBar;
        public TMP_Text _name;

        public GameObject UIText;
        public Transform SpwanUIText;

        private int _totalDamage;


   




        public override void Attached()
        {
            if (!entity.IsOwner) return;

            GO = GetComponent<BoltEntity>();
            state.AddCallback("CurrentHealth", CurrentHealthCallBack);//Обратный вызов Текущее здоровье
            state.AddCallback("TotalHealth", MaxHealthCallBack);//Обратный вызов максимальное здоровье
            SetHealthPlayer();
            UpdateUIMaxHealth();
           
            

        }

        private void CurrentHealthCallBack()
        {
            //if (entity.IsOwner)
            //{
            //    localHealth = state.CurrentHealth;
            //    UpdateUIPlayer();

            //    if (localHealth <= 0)
            //    {
            //        state.CurrentHealth = 0;
            //        state.DeathTrigger();
            //        state.Dead = true;
            //        _ = DeadPlayer.Post(GO, true);

            //        //BoltNetwork.Destroy(gameObject);
            //    }
            //}
        }

        private void MaxHealthCallBack()
        {

            if (!entity.IsOwner) return; 
            
            _maxHealth = state.TotalHealth;
            UpdateUIPlayer();
            

        }

        public void AnimationDamage(string damage)
        {
            if (!entity.IsOwner && !state.Dead)
            {
               GameObject Damage =  Instantiate(UIText, SpwanUIText);
               Damage.GetComponent<TMP_Text>().text = damage;
             
            }



        }

        private void FixedUpdate()
        {
            UpdateUIPlayer();
        }

        private void SetHealthPlayer()//Загрузка здоровья персонажа
        {

            localHealth = _maxHealth;
            state.TotalHealth = _maxHealth;
            state.CurrentHealth = localHealth;

        }



        private void UpdateUIMaxHealth()//Обновление максимального слайдера - максимальное здоровье
        {
            healthBar.maxValue = _maxHealth;
        }

        private void UpdateUIPlayer()//обновление UI на персонаже
        {
            _name.text = entity.GetState<IPlayer>().Name;
            healthBar.maxValue = entity.GetState<IPlayer>().TotalHealth;
            healthBar.value = entity.GetState<IPlayer>().CurrentHealth;
        }

        public override void OnEvent(SetDamage evnt)
        {

            if (evnt.Target.IsOwner&&!evnt.InfoMessage)
            {
                if (!state.Dead)
                {
                    if (state.Armor > 0)
                    {
                         _totalDamage = (int)(evnt.Damage);
                    }
                    else
                    {
                        _totalDamage = evnt.Damage;

                    }

                    entity.GetState<IPlayer>().CurrentHealth -= _totalDamage;

                    SetDamage.Post(evnt.OwnerEntity, _totalDamage, evnt.OwnerEntity, true,evnt.Target);// Отправка сообщения игроку который нанес урон, для анимации

                    if (_totalDamage < 1)
                    {
                       
                        _ = LogEvent.Post(entity, EntityTargets.OnlySelf, string.Format("<color=#DC143C>Вы блокировали удар! </color>"), false);
                       

                    }
                    else
                    {
                      _ = LogEvent.Post(entity, EntityTargets.OnlySelf, string.Format("<color=#DC143C>Вам нанесли урон {0} </color>", _totalDamage), false);

                       
                    }

                }
            }
        }

       
        public override void OnEvent(SpecialActiveShield evnt)
        {
            if (evnt.FromSelf)
            {
                BoltLog.Warn("Активирован Щит" + evnt.Value);
                state.Armor = evnt.Value;
            }
           
        }

        


    }
}