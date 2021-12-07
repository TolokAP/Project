
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Player
{
    public enum WeaponHandPosition // Позиция - левая, правая рука. Список.
    {
        None=1,
        Left ,
        Right,
        Player
      
    }
    public enum SpecialAttack
    {
        none,
        doubleDamage ,
        activeShield,
        stun,
      
        

    }
    public class CombatSystem : EntityEventListener<IPlayer>
    {
        public Button SpecialBtn;
        public Button WarModeBtn;



        [SerializeField]
        private GameObject _HandR;




        [SerializeField]
        public int _attackDistance { get; set; }//Дистанция атаки
        public CombatsSkills combatsSkills;
       


        public GameObject TargetGameObject;// Текущая цель
        public BoltEntity TargetEntity;//Сущность текущей цели
       
        public List<GameObject> TargetGO = new List<GameObject>();// Список Таргетов игрока
        private int targetNumber;

        public GameObject TargetGroup;//UI эллемент цели игрока
        public GameObject SpawnPositionArcherWeapon;// Позиция создания стрелы для лучника

        private UITarget _UITarget;// Ссылка на скрипт обновления. цели игрока UI эллементов 
        public float[] dropChance;


        private ScriptMethodsSkill MethodsSkill;
        
        [SerializeField]
        private int[] _damage = new int[2];

        [SerializeField]
        private GameObject _ForceShield;


        public GameObject target;
    


      public void SetParametrOfWeapon(ItemData item,GameObject weapon)// Смена параметров оружия, при смете в экпировке(Дистанция, Умение, Урон, Наличие Оружия)
        {
            if (entity.IsOwner)
            {
                _attackDistance = (int)item.attackDistance;
                combatsSkills = item.skill;
                state.CombatSkill = combatsSkills.GetHashCode();
              
                               
                for (int i = 0; i < _damage.Length; i++)
                {
                    _damage[i] = item.damage[i];

                }
                _HandR = weapon;
            }
        }

       

      


       
        public override void Attached()
        {
            if(BoltNetwork.IsClient){
                if (entity.IsOwner)
                {

                    SetButton();
                  
                    targetNumber = 0;

                    state.SetTransforms(state.Transform, transform);
                    state.SetAnimator(GetComponent<Animator>());

                    state.Animator.applyRootMotion = entity.IsOwner;

                    state.Animationspeed = 1f;

                  

                }
            }
        }

        public override void SimulateOwner()
        {

        }

        #region События Photon Bolt

        public override void OnEvent(SetDamage evnt)
        {
            if (evnt.Target.IsOwner&& evnt.InfoMessage)
            {
                if (evnt.Damage >= 1)
                {
                    TargetGameObject.GetComponent<Health>().AnimationDamage(evnt.Damage.ToString());
              
                }
                else
                {
                
                    TargetGameObject.GetComponent<Health>().AnimationDamage("Блок");
                }



            }
        }

        public override void OnEvent(SpecialActiveShield evnt)
        {
            _ForceShield.SetActive(evnt.ForceShield);

        }
        public override void OnEvent(EventSpecialAttack evnt)
        {
            if (evnt.TargEntity.IsOwner)
            {
                if (!state.Stunned)
                {
                    state.StunTrigger();
                    state.Stunned = true;
                    StartCoroutine(StopStunCoroutine());
                }
            }
        }

        #endregion





        #region AnimationEvent

        public void AttackStart() //Событие анимации. Начало атаки
        {
            BoltLog.Warn("Старт Атакки");
            if (entity.IsOwner) { state.Attack = true; GetComponentInChildren<WeaponsScript>().Hit(); }
        }

        public void AttackFinish()//Событие анимации. Окончание атаки
        {
            if (entity.IsOwner)
            {
                UpSkill(combatsSkills);
                BoltLog.Warn("Конец Атаки");
                state.Attack = false;

                //if (GetDistanceToTarget() < _attackDistance&& !state.Moving)
                //{
                //    Attack();
                //    BoltLog.Warn("Повторная атака");
                //}
            }
        }

        public void Hit(int multiplicator)//Событие анимации Атаки - Вариор
        {
            if (entity.IsOwner)
            {
                int value=GetDamage(multiplicator);

                SetDamage.Post(TargetEntity, value,TargetEntity,false,entity);
                             

                BoltLog.Warn("Атака");
              

            }
        }
       public void Buf()//Специальное умение Щит
        {

            if (entity.IsOwner)
            {
                BoltLog.Warn("Защита");
                SpecialActiveShield.Post(entity, 0.01f,true);
                _ForceShield.SetActive(true);
                StartCoroutine(StopBufCoroutine());
            }
        }

        public void Stuned()//Специальное умение Оглушение
        {
            if (entity.IsOwner)
            {
                BoltLog.Warn("Оглушение");
                EventSpecialAttack.Post(TargetEntity, true,TargetEntity);
             
            }



        }

        public void Shoot(int a)//Событие анимации атаки - Лучник
        {
            if (entity.IsOwner)
            {
               

               
                var _TokenPosition = new TargetPosition
                {
                    NetworkID = TargetGameObject.GetComponent<BoltEntity>().NetworkId.PackedValue,
                    NetworkIDOwner = entity.NetworkId.PackedValue,
                    Damage = GetDamage(a)

                };
                //GameObject Arrow = BoltNetwork.Instantiate(BoltPrefabs.Arrow, _TokenPosition, SpawnPositionArcherWeapon.transform.position, Quaternion.LookRotation(TargetGameObject.transform.position));
                //Arrow.transform.LookAt(TargetGameObject.transform);
                Debug.Log("Атака Дальнего боя");

            }



        }



        #endregion
        #region Корутины
        IEnumerator StopBufCoroutine()//Остановка действия щита 
        {

            yield return new WaitForSeconds(10f);
            _ForceShield.SetActive(false);
            SpecialActiveShield.Post(entity, 1f,false);
            yield break;
        }
        IEnumerator StopStunCoroutine()//Время действия специальной аттаки оглушение
        {

            yield return new WaitForSeconds(5f);
            
            state.Stunned = false;
        
            yield break;
        }


        #endregion


        #region Атака
        private bool CheckOpportunityAttack()// Проверяет: Владельца сущности, есть ли обьект атаки, Фаза атаки, дистанцию до цели, живая цель, не умер игрок, режим атаки
        {
            if (entity.IsOwner &&
                TargetGameObject != null &&
                !state.Attack &&
                GetDistanceToTarget() <= _attackDistance &&
                !TargetEntity.GetState<IPlayer>().Dead &&
                !state.Dead &&
                 state.AttackMode
                 &&!state.Stunned)
            {
                return true;
            }
            else return false;
        }
        private bool CheckPosibilityActiveShield()//проверка возмодности использовать спецаильное умение (Щит)
        {
            if (entity.IsOwner &&
                !state.Dead && !
                !state.Stunned) return true;
            else return false;
        }
       

        public void Attack()// Метод аттаки. Установлен на кнопке
        {
            if (entity.IsOwner)
            {
                if (_HandR == null)
                {
                    LogEvent.Post(entity, "Нет оружия", true);

                    return;
                }
                if (CheckOpportunityAttack())
                {
                    state.CombatSkill = combatsSkills.GetHashCode();
                    state.Animationspeed = (float)System.Math.Round(state.Skills[state.CombatSkill] / 100, 2);
                    transform.LookAt(TargetGameObject.transform);
                    state.AttackTrigger();
                
                }
            }

        }


        private int GetDamage(int damage) // Формула получения дамага
        {
            int value;
            value = damage * Random.Range(_damage[0], _damage[1]);

            return value;

        }
        private void UpSkill(CombatsSkills skill)//Шанс роста скилла
        {
            if (MethodsSkill.SetChanceUpSkill((float)System.Math.Round(state.Skills[combatsSkills.GetHashCode()] / 100, 1)))
            {
                BoltLog.Warn("Рост умения, отправка события");
                _ = ChangeSkill.Post(entity, skill.GetHashCode(), 0.1f);
            }

        }



        #endregion
        #region Специальные аттаки

        private void SpecialAttack1()//Метод на кнопке 1
        {
            OnSpecialAttack(GetNumberSpecialAttack(state.SpecialAttack[0].AttackNumber));

        }
        private void SpecialAttack2()//Метод на кнопке 2
        {
            OnSpecialAttack(GetNumberSpecialAttack(state.SpecialAttack[1].AttackNumber));
        }
        private void SpecialAttack3()//Метод на кнопке 3
        {
            OnSpecialAttack(GetNumberSpecialAttack(state.SpecialAttack[2].AttackNumber));

        }

        private void OnSpecialAttack(SpecialAttack specialAttack)
        {
            switch (specialAttack)
            {
                case SpecialAttack.doubleDamage:
                    if(CheckOpportunityAttack())SAttack(specialAttack.GetHashCode());
                    break;
                case SpecialAttack.activeShield:
                    SAttack(specialAttack.GetHashCode());
                    break;
                case SpecialAttack.stun:
                    if (CheckOpportunityAttack()) SAttack(specialAttack.GetHashCode());
                    break;

            }

        }

        private SpecialAttack GetNumberSpecialAttack(int number)
        {
            SpecialAttack special = (SpecialAttack)number;
            return special;


        }

        private void SAttack(int number)
        {
            state.ComboNumber = number;
            state.SpecialAttackTrigger();
            //if (entity.IsOwner)
            //{
            //    if (_HandR == null)
            //    {
            //        LogEvent.Post(entity, "Нет оружия", true);

            //        return;
            //    }
            //    if (CheckOpportunityAttack())
            //    {
                   
            //        transform.LookAt(TargetGameObject.transform);
            //        state.ComboNumber = number;
            //        state.SpecialAttackTrigger();
            //    }
            //}
       
        }


        #endregion


        #region Управление таргетом игрока



        private void ReloadTargetGO()// заполнение массива обьектов в зоне видимости Игрока
        {
            if (entity.IsOwner)
            {
                Collider[] _col = Physics.OverlapSphere(transform.position, 10f);
                TargetGO.Clear();
                foreach (var col in _col)
                {
                    if (col.CompareTag("Player") && !TargetGO.Contains(col.gameObject))
                    {
                        if (!col.GetComponent<BoltEntity>().IsOwner)
                        {
                            TargetGO.Add(col.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("Обьект уже есть ");
                    }
                }
            }

        }

        private void ActiveTarget() //Активирует Particle Цели, и включает табло отражения.
        {
            
                if (TargetGameObject != null)
                {
                    ParticleSystem(TargetGameObject, true);
                    TargetGroup.SetActive(true);
                    if (TargetEntity.isActiveAndEnabled)
                    {
                        _UITarget.UpdateUI(TargetEntity.GetState<IPlayer>().CurrentHealth, TargetEntity.GetState<IPlayer>().TotalHealth, TargetEntity.GetState<IPlayer>().Name);
                    }

                }
            




        }

        private void ChangeTarget() // переключение таргета между целями
        {
            ReloadTargetGO();

            if (TargetGO.Count != 0)
            {
                targetNumber++;
                Debug.Log("Текущий таргет " + targetNumber);
                if (targetNumber > TargetGO.Count - 1)
                {
                    targetNumber = 0;

                }
                foreach (GameObject Go in TargetGO)
                {
                    ParticleSystem(Go, false);
                }
                TargetGameObject = TargetGO[targetNumber];
                TargetEntity = TargetGameObject.GetComponent<BoltEntity>();
                ActiveTarget();

            }
            else
            {
                targetNumber = 0;
            }

        }

        private float GetDistanceToTarget()//Определение дистанции до цели
        {
            return Vector3.Distance(transform.position, TargetGameObject.transform.position);
        }






        #endregion

        private void ParticleSystem(GameObject obj, bool status)
        {
            BoltLog.Warn("Партикл включить у обьекта " + obj.name);
            if (obj.CompareTag("Player"))
            {
              obj.GetComponent<CombatSystem>().target.SetActive(status);
              
            }
        }

        public void WarMode()// Переключатель в боевой и мирный режим.
        {
            if (state.AttackMode)
            {
                WarModeBtn.GetComponent<Image>().color = Color.white;
                BoltLog.Warn("WarMode = false");
                state.AttackMode = false;
       
            }
            else
            {
                state.AttackMode = true;
                WarModeBtn.GetComponent<Image>().color = Color.red;
                BoltLog.Warn("WarMode = true");

            }

        }


        private void OnTriggerEnter(Collider other)//Вхождение целей в триггер
        {

            //BoltLog.Warn("Входит в триггер игрока " + other.name);

            if (other.gameObject == TargetGameObject)
            {
                ActiveTarget();
            }



        }

        private void OnTriggerExit(Collider other)//Выход целей из триггера
        {
            if (other.gameObject == TargetGameObject)
            {
                if (TargetGameObject != null)
                {

                    ParticleSystem(TargetGameObject, false);
                    TargetGroup.SetActive(false);

                }
            }

            //BoltLog.Warn("Выходит из триггера игрока" + other.name);
        }

       

       
       


        private void SetButton()//Получение ссылок на UI обьекты.
        {
          

            TargetGroup = GameObject.FindGameObjectWithTag("TargetGroup");
            _UITarget = TargetGroup.GetComponent<UITarget>();

         
            GameObject go = GameObject.FindGameObjectWithTag("ActionButtonGroup");
            Button []button = go.GetComponentsInChildren<Button>();
            foreach(Button value in button) { 
                switch (value.name)
                {
                    case "AttackBtn":
                        value.onClick.AddListener(delegate { Attack(); });
                        break;
                    case "ChangeTargetBtn":
                        value.onClick.AddListener(delegate { ChangeTarget(); });
                        break;
                    case "WarMode":
                        WarModeBtn = value;
                        value.onClick.AddListener(delegate { WarMode(); });
                        break;
                    case "SpecialAttackBtn1":
                        SpecialBtn = value;
                        value.onClick.AddListener(delegate { SpecialAttack1(); });
                        break;
                    case "SpecialAttackBtn2":
                        SpecialBtn = value;
                        value.onClick.AddListener(delegate { SpecialAttack2(); });
                        break;
                    case "SpecialAttackBtn3":
                        SpecialBtn = value;
                        value.onClick.AddListener(delegate { SpecialAttack3(); });
                        break;

                }
            }

            

        
        }

      
    }
}
