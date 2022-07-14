using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine;

namespace Player
{
    public class DressingEquipInPlayer : EntityEventListener<IPlayer>
    {
        public ItemDatabase itemDatabase;

        public GameObject[] EquipID;//������ ��������� ����������
        public Material[] material;// ������ ��������� ���������� ��� ����������
      


        public GameObject HandL;//����� ����
        public GameObject HandR;//������ ����

        public int[] _equipmentID;

        [SerializeField]
        private CombatSystem _combatsystem;

        public override void Attached()
        {
          
            _combatsystem = gameObject.GetComponentInParent<CombatSystem>();
            state.AddCallback("Equipmnet[]", ChangeEquipment);
            _equipmentID = new int[state.Equipmnet.Length];
           

        }

       




        private void ResetItemEquipInPlayer(int index)//��������/���������� ��������� � ��������� � �� ����
        {

            if (state.Equipmnet[index].ID != 0) return;
            
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

            if (!EquipID[index].TryGetComponent<BoltEntity>(out BoltEntity boltEntity)) return;
            
            BoltNetwork.Destroy(EquipID[index]);
                    
            
           


            

        }

        private void OnItemInPlayer(int index,int indexMat)
        {
            if (EquipID[index].activeSelf) return;
            
                EquipID[index].SetActive(true);
                EquipID[index].GetComponent<Renderer>().material = material[indexMat];
            

        }
        private void  OffItemInPlayer(int index)
        {
            if (!EquipID[index].activeSelf) return;
            
                EquipID[index].SetActive(false);
            

        }

        public void ChangeEquipment(IState istate, string propertPath, ArrayIndices arrayIndices)//��������� ����� ���������� ������
        {

           
            if (propertPath == "Equipmnet[].quantity") return;//���� �������� ���������� ���������� ���������� �������


            int index = arrayIndices[0];
            ItemData ItemData = itemDatabase.LookIDItem(state.Equipmnet[index].ID);

            if (state.Equipmnet[index].ID != 0) //���� ���� �������� 0 ������� �������
            {
              
                if (entity.IsOwner)
                {
                    _equipmentID[index] = state.Equipmnet[index].ID;
                    state.Armor += ItemData.GetArmorItem;
                }
                switch (ItemData.GetTypeEquipment)
                    {
                        case TypeEquipment.weapon: //������ ����� ����
                       
                        if (!entity.IsOwner) return;
                            
                                if (HandR != null) //��������� �� ������� ������ � ����
                                {
                                    BoltNetwork.Destroy(HandR);
                                }
                                var networkIDR = new NetworkIDToken
                                {
                                    NetworkID = entity.NetworkId.PackedValue,
                                    Hand = (int)WeaponHandPosition.Right,

                                };
                                HandR = BoltNetwork.Instantiate(ItemData.GetPrefabItem, networkIDR);
                                EquipID[TypeEquipment.weapon.GetHashCode()] = HandR;
                                _combatsystem.SetParametrOfWeapon(ItemData, HandR);
                                state.CombatSkill = ItemData.GetCombatsSkill.GetHashCode();
                                state.SpecialAttack[0].AttackNumber = ItemData.GetSpecialAttack.GetHashCode();
                                state.Damage[0] = ItemData.GetMinDamageWeapon;
                                state.Damage[1] = ItemData.GetMaxDamageWeapon;

                        
                            break;
                        case TypeEquipment.twoHandWeapon: //������ ������ ����
                              
                        if (!entity.IsOwner) return;
                            
                                if (HandL != null)//��������� �� ������� ������ � ����
                                {
                                    BoltNetwork.Destroy(HandL);
                                }
                                var networkIDL = new NetworkIDToken
                                {

                                    NetworkID = entity.NetworkId.PackedValue,
                                    Hand = (int)WeaponHandPosition.Left
                                };

                                HandL = BoltNetwork.Instantiate(ItemData.GetPrefabItem, networkIDL);
                                EquipID[TypeEquipment.twoHandWeapon.GetHashCode()] = HandL;
                                BoltLog.Warn("�������� ������ ����� ����" + ItemData.GetSpecialAttack.GetHashCode());
                                state.SpecialAttack[1].AttackNumber = ItemData.GetSpecialAttack.GetHashCode();
                                
                            
                            break;
                        default: //��� ��������� �������� ����������
                        OnItemInPlayer(ItemData.GetTypeEquipment.GetHashCode(),ItemData.GetTypeMaterial.GetHashCode());
                        break;
                    }

            }
            else
            {
                


                if (entity.IsOwner)
                {
                    ItemData itemData = itemDatabase.LookIDItem(_equipmentID[index]);
                    _equipmentID[index] = 0;
                    state.Armor -= itemData.GetArmorItem;
                    switch (itemData.GetTypeEquipment)
                    {
                        case TypeEquipment.weapon:

                            BoltLog.Error("������� ������");
                            state.Damage[0] = 0;
                            state.Damage[1] = 0;

                            if (EquipID[3] == null) return;

                            BoltNetwork.Destroy(EquipID[3]);
                           

                            break;
                            case TypeEquipment.twoHandWeapon:

                            if (EquipID[4] == null) return;

                            BoltNetwork.Destroy(EquipID[4]);

                            break;


                    }
                    
                  
                }
                if (index == 3 || index == 4) return;
                OffItemInPlayer(index);
       

            }
        }
        
       
       

    }
}
