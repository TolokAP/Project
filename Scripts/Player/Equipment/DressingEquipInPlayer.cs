using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine;

namespace Player
{
    public class DressingEquipInPlayer : EntityEventListener<IPlayer>
    {

        public GameObject[] EquipID;//������ ��������� ����������
        public Material[] material;// ������ ��������� ���������� ��� ����������
        public ItemDB _itemDB; //������ �� ������ ���� ���� ��������� 


        public GameObject HandL;//����� ����
        public GameObject HandR;//������ ����

     

        [SerializeField]
        private CombatSystem _combatsystem;

        public override void Attached()
        {
           
            _itemDB = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDB>();
            _combatsystem = gameObject.GetComponentInParent<CombatSystem>();
            state.AddCallback("Equipmnet[]", ChangeEquipment);
          
        }

      
        

        public ItemData lookUpID(int ID)// ������� ������ �� ���� ���������
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

        private void ResetItemEquipInPlayer(int index)//��������/���������� ��������� � ��������� � �� ����
        {

            if (state.Equipmnet[index].ID == 0)
            {
                BoltLog.Warn("���������� ����� ������ " + index);
                switch (index)
                {
                    case 3:
                        BoltLog.Warn("������ ������ 4 ");
                        state.SpecialAttack[0].AttackNumber = 0;
                        break;
                    case 4:
                        BoltLog.Warn("������ ������ 5 ");
                        state.SpecialAttack[1].AttackNumber = 0;
                        break;
                    case 5:
                        BoltLog.Warn("������ ������ 6 ");
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
        public void ChangeEquipment(IState istate, string propertPath, ArrayIndices arrayIndices)//��������� ����� ���������� ������
        {
          
                //BoltLog.Warn("�������� ����� ����� ���������� �� ������� CombatSystem");
                int index = arrayIndices[0];

               BoltLog.Warn("�������� ����� ����� ���������� " + index);
            if (propertPath == "Equipmnet[].quantity") return;

                ItemData ItemData = lookUpID(state.Equipmnet[index].ID);
                //BoltLog.Warn("������" + state.Equipmnet[index].ID);
                //BoltLog.Warn("������ �������" + EquipID.Length);
          

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
                        BoltLog.Warn("�������� ������ ����� ����" + ItemData.specialAttack.GetHashCode());
                        state.SpecialAttack[1].AttackNumber = ItemData.specialAttack.GetHashCode();
                        break;
                    case TypeEquipment.gloves:

                        DressingEquipment.Post(entity, 5, 0,true);
                        BoltLog.Warn("�������� ������ ����� ��������" + ItemData.specialAttack.GetHashCode());
                        state.SpecialAttack[2].AttackNumber = ItemData.specialAttack.GetHashCode();

                        break;
                    case TypeEquipment.chest:

                        DressingEquipment.Post(entity, 1, ItemData.materialID, true);

                        break;

                }
            }
        }

        #region Bolt_Event
        public override void OnEvent(DressingEquipment evnt)//������� ��������� ��������� ���������� �� ���������
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
