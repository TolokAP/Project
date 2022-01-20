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
        
     

        [SerializeField]
        private CombatSystem _combatsystem;

        public override void Attached()
        {
           
          
            _combatsystem = gameObject.GetComponentInParent<CombatSystem>();
            state.AddCallback("Equipmnet[]", ChangeEquipment);
          
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

           
            int index = arrayIndices[0];

               BoltLog.Warn("�������� ����� ����� ���������� " + index);
            if (propertPath == "Equipmnet[].quantity") return;
                       

            //BoltLog.Warn("������" + state.Equipmnet[index].ID);
            //BoltLog.Warn("������ �������" + EquipID.Length);

            if (entity.IsOwner)
            {
                ResetItemEquipInPlayer(index);
                if (state.Equipmnet[index].ID == 0) return;//���� ���� �������� 0 ������� �������
                ItemData ItemData = itemDatabase.LookIDItem(state.Equipmnet[index].ID);
                switch (ItemData.GetTypeEquipment)
                {

                    case TypeEquipment.weapon:

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
                        EquipID[3] = HandR;
                        _combatsystem.SetParametrOfWeapon(ItemData, HandR);
                       
                        state.SpecialAttack[0].AttackNumber = ItemData.GetSpecialAttack.GetHashCode();

                        break;
                    case TypeEquipment.twoHandWeapon:
                       
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
                        EquipID[4] = HandL;
                        BoltLog.Warn("�������� ������ ����� ����" + ItemData.GetSpecialAttack.GetHashCode());
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
