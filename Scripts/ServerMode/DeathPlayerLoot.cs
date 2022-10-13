
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Bolt.Utils;
using Photon.Bolt;
using System.Collections.Generic;
using Newtonsoft.Json;
using DuloGames.UI;
using Player;
using UnityEngine.UI;


public class DeathPlayerLoot : EntityBehaviour<IDeadPlayerState>
{
    [SerializeField]
    private List<int> idItem = new List<int>();

    public bool _isOpen;

    public GameObject PlayerWhoOpen;

    

    public override void Attached()
    {
        var token = (LootEquipID)entity.AttachToken;
        idItem = JsonConvert.DeserializeObject<List<int>>(token.EquipID); //��������� ������ ��������� ������� ���������� �������

        for(int i = 0; i < idItem.Count; i++)//������� ������ � ��������� ��������
        {
            state.IdItem[i] = idItem[i];
        }

       
        state.AddCallback("IdItem[]", ChangeItem); // �������� ����� ��������� ������ ��������� ��� ����


    }

    private void ChangeItem(IState states, string propertyPath, ArrayIndices arrayIndices) // �������� ����� ��������� ������ ��������� ��� ����
    {
        int index = arrayIndices[0];
        if (!_isOpen) return;
        if (PlayerWhoOpen != null) {

            PlayerWhoOpen.GetComponent<ActionHandScript>().InstantiateItems(state.IdItem, entity);//�������� �������� � ��������� ��� ������ ����

        }
       
      
    }

  

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerWhoOpen)
        {
            BoltLog.Error("����� �� ��������");
           _isOpen = false;
            PlayerWhoOpen.GetComponent<ActionHandScript>()._lootWindow.Hide();
        }
    }

   
}
