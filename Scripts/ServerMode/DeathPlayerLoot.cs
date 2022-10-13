
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
        idItem = JsonConvert.DeserializeObject<List<int>>(token.EquipID); //Получение списка предметов которые необходимо создать

        for(int i = 0; i < idItem.Count; i++)//Перенос данных в состояние сущности
        {
            state.IdItem[i] = idItem[i];
        }

       
        state.AddCallback("IdItem[]", ChangeItem); // обратный вызов обновляет список предметов для лута


    }

    private void ChangeItem(IState states, string propertyPath, ArrayIndices arrayIndices) // обратный вызов обновляет список предметов для лута
    {
        int index = arrayIndices[0];
        if (!_isOpen) return;
        if (PlayerWhoOpen != null) {

            PlayerWhoOpen.GetComponent<ActionHandScript>().InstantiateItems(state.IdItem, entity);//Обновить предметы у персонажа кто октрыл труп

        }
       
      
    }

  

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerWhoOpen)
        {
            BoltLog.Error("Вышел из триггера");
           _isOpen = false;
            PlayerWhoOpen.GetComponent<ActionHandScript>()._lootWindow.Hide();
        }
    }

   
}
