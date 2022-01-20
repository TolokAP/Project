using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine;

namespace Player
{
    public class DressingEquipInPlayer : EntityEventListener<IPlayer>
    {
        public ItemDatabase itemDatabase;

        public GameObject[] EquipID;//Массив элементов экипировки
        public Material[] material;// Массив элементов материалов для экипировки
      


        public GameObject HandL;//Левая рука
        public GameObject HandR;//Правая рука
        
     

        [SerializeField]
        private CombatSystem _combatsystem;

        public override void Attached()
        {
           
          
            _combatsystem = gameObject.GetComponentInParent<CombatSystem>();
            state.AddCallback("Equipmnet[]", ChangeEquipment);
          
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

           
            int index = arrayIndices[0];

               BoltLog.Warn("Работает метод смены экипировки " + index);
            if (propertPath == "Equipmnet[].quantity") return;
                       

            //BoltLog.Warn("Индекс" + state.Equipmnet[index].ID);
            //BoltLog.Warn("Размер Индекса" + EquipID.Length);

            if (entity.IsOwner)
            {
                ResetItemEquipInPlayer(index);
                if (state.Equipmnet[index].ID == 0) return;//Если айди предмета 0 удалить предмет
                ItemData ItemData = itemDatabase.LookIDItem(state.Equipmnet[index].ID);
                switch (ItemData.GetTypeEquipment)
                {

                    case TypeEquipment.weapon:

                        if (HandR != null) //Проверяет на наличие оружия в руке
                        {
                            BoltNetwork.Destroy(HandR);
                        }
                        var networkIDR = new NetworkIDToken
                        {
                            NetworkID = entity.NetworkId.PackedValue,
                            Hand = (int)WeaponHandPosition.Right,

                        };
                        HandR = BoltNetwork.Instantiate(ItemData.GetPrefabItem, networkIDR);
                        EquipID[3] = HandR;
                        _combatsystem.SetParametrOfWeapon(ItemData, HandR);
                       
                        state.SpecialAttack[0].AttackNumber = ItemData.GetSpecialAttack.GetHashCode();

                        break;
                    case TypeEquipment.twoHandWeapon:
                       
                        if (HandL != null)//Проверяет на наличие оружия в руке
                        {
                            BoltNetwork.Destroy(HandL);
                        }
                        var networkIDL = new NetworkIDToken
                        {
                            
                        NetworkID = entity.NetworkId.PackedValue,
                            Hand = (int)WeaponHandPosition.Left
                        };

                        HandL = BoltNetwork.Instantiate(ItemData.GetPrefabItem, networkIDL);
                        EquipID[4] = HandL;
                        BoltLog.Warn("Работает методи смены щита" + ItemData.GetSpecialAttack.GetHashCode());
                        state.SpecialAttack[1].AttackNumber = ItemData.GetSpecialAttack.GetHashCode();
                        break;
                   
                    case TypeEquipment.helmet:

                        DressingEquipment.Post(entity, 0, ItemData.GetTypeMaterial.GetHashCode(), true);
                        break;
                    case TypeEquipment.chest:

                        DressingEquipment.Post(entity, 1, ItemData.GetTypeMaterial.GetHashCode(), true);

                        break;
                    case TypeEquipment.legs:

                        DressingEquipment.Post(entity, 2, ItemData.GetTypeMaterial.GetHashCode(), true);

                        break;
                    case TypeEquipment.gloves:

                        DressingEquipment.Post(entity, 5, ItemData.GetTypeMaterial.GetHashCode(), true);
                   
                        state.SpecialAttack[2].AttackNumber = ItemData.GetSpecialAttack.GetHashCode();

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
