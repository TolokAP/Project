using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine;

namespace Player
{
    public class DressingEquipInPlayer : EntityEventListener<IPlayer>
    {

        public GameObject[] EquipID;//Массив элементов экипировки
        public Material[] material;// Массив элементов материалов для экипировки
        public ItemDB _itemDB; //Ссылка на массив базы всех предметов 


        public GameObject HandL;//Левая рука
        public GameObject HandR;//Правая рука

     

        [SerializeField]
        private CombatSystem _combatsystem;

        public override void Attached()
        {
           
            _itemDB = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDB>();
            _combatsystem = gameObject.GetComponentInParent<CombatSystem>();
            state.AddCallback("Equipmnet[]", ChangeEquipment);
          
        }

      
        

        public ItemData lookUpID(int ID)// вернуть обьект из базы предметов
        {
            foreach (ItemData ItemData in _itemDB.itemDatabase)
            {
                if (ItemData.ID == ID)
                {
                    return ItemData;
                }
            }
            return null;

        }

        private void ResetItemEquipInPlayer(int index)//Удаление/Отключение предметов с персонажа и из сети
        {

            if (state.Equipmnet[index].ID == 0)
            {
                BoltLog.Warn("Посмотреть какой индекс " + index);
                switch (index)
                {
                    case 3:
                        BoltLog.Warn("Меняем индекс 4 ");
                        state.SpecialAttack[0].AttackNumber = 0;
                        break;
                    case 4:
                        BoltLog.Warn("Меняем индекс 5 ");
                        state.SpecialAttack[1].AttackNumber = 0;
                        break;
                    case 5:
                        BoltLog.Warn("Меняем индекс 6 ");
                        state.SpecialAttack[2].AttackNumber = 0;
                        break;

                }

                if (EquipID[index].TryGetComponent<BoltEntity>(out BoltEntity boltEntity))
                {
                    BoltNetwork.Destroy(EquipID[index]);
                    
                }
                else
                {
                    DressingEquipment.Post(entity, index, 0,false);
                 
                }


            }

        }
        public void ChangeEquipment(IState istate, string propertPath, ArrayIndices arrayIndices)//изменение слота экипировки оружия
        {
          
                //BoltLog.Warn("Работает метод смены экипировки из скрипта CombatSystem");
                int index = arrayIndices[0];

               BoltLog.Warn("Работает метод смены экипировки " + index);
            if (propertPath == "Equipmnet[].quantity") return;

                ItemData ItemData = lookUpID(state.Equipmnet[index].ID);
                //BoltLog.Warn("Индекс" + state.Equipmnet[index].ID);
                //BoltLog.Warn("Размер Индекса" + EquipID.Length);
          

            if (entity.IsOwner)
                {
                ResetItemEquipInPlayer(index);
                switch (ItemData.type)
                {

                    case TypeEquipment.weapon:


                        var networkIDR = new NetworkIDToken
                        {
                            NetworkID = entity.NetworkId.PackedValue,
                            Hand = (int)WeaponHandPosition.Right,

                        };
                        HandR = BoltNetwork.Instantiate(ItemData.prefab, networkIDR);
                        EquipID[3] = HandR;
                        _combatsystem.SetParametrOfWeapon(ItemData, HandR);
                       
                        state.SpecialAttack[0].AttackNumber = ItemData.specialAttack.GetHashCode();

                        break;
                    case TypeEquipment.twoHandWeapon:
                        var networkIDL = new NetworkIDToken
                        {
                            NetworkID = entity.NetworkId.PackedValue,
                            Hand = (int)WeaponHandPosition.Left
                        };

                        HandL = BoltNetwork.Instantiate(ItemData.prefab, networkIDL);
                        EquipID[4] = HandL;
                        BoltLog.Warn("Работает методи смены щита" + ItemData.specialAttack.GetHashCode());
                        state.SpecialAttack[1].AttackNumber = ItemData.specialAttack.GetHashCode();
                        break;
                    case TypeEquipment.gloves:

                        DressingEquipment.Post(entity, 5, 0,true);
                        BoltLog.Warn("Работает методи смены перчаток" + ItemData.specialAttack.GetHashCode());
                        state.SpecialAttack[2].AttackNumber = ItemData.specialAttack.GetHashCode();

                        break;
                    case TypeEquipment.chest:

                        DressingEquipment.Post(entity, 1, ItemData.materialID, true);

                        break;

                }
            }
        }

        #region Bolt_Event
        public override void OnEvent(DressingEquipment evnt)//Событие включение элементов экипировке на персонаже
        {
            if (evnt.Active)
            {
                EquipID[evnt.EquipID].SetActive(true);
                EquipID[evnt.EquipID].GetComponent<Renderer>().material = material[evnt.MaterialID];
            }
            else
            {
                EquipID[evnt.EquipID].SetActive(false);
            }
        }


        #endregion

    }
}
