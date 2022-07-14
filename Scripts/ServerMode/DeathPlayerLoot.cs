
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Bolt.Utils;
using Photon.Bolt;
using System.Collections.Generic;
using Newtonsoft.Json;
using DuloGames.UI;
using Player;
using UnityEngine.UI;
using System;

public class DeathPlayerLoot : EntityBehaviour<IDeadPlayerState>, IPointerDownHandler
{
    public List<int> idItem = new List<int>();
    public GameObject LootBox;
    public ItemDatabase itemDatabase;
    public GameObject LootItem;
    public GameObject ContentBox;
    [SerializeField]
    private GameObject _Player;
    [SerializeField]
    private BoltEntity _entityDeathPlayer;
    [SerializeField]
    private BoltEntity _entityPlayers;

    public List<GameObject> Items = new List<GameObject>();
 
    public override void Attached()
    {
        var token = (LootEquipID)entity.AttachToken;
        idItem = JsonConvert.DeserializeObject<List<int>>(token.EquipID); //Получение списка предметов которые необходимо создать

        for(int i = 0; i < idItem.Count; i++)//Перенос данных в состояние сущности
        {
            state.IdItem[i] = idItem[i];
        }


        LootBox = GameObject.FindGameObjectWithTag("LootBox");// Получение ссылки на место куда переносить предметы

        FindEntityPlayer();
        _entityDeathPlayer = gameObject.GetComponentInParent<BoltEntity>();

        ContentBox = LootBox.GetComponentInChildren<GridLayoutGroup>().gameObject;

        NetworkId network = new NetworkId(token.NetworkID);
        var _entity = BoltNetwork.FindEntity(network);
        BoltLog.Error("Угол поворота " + _entity.transform.rotation);
        Instantiate(_entity.gameObject.GetComponent<PlayerState>().body, _entity.transform.position, _entity.transform.rotation,gameObject.transform);

        state.AddCallback("IdItem[]", ChangeItem);
    }

    private void ChangeItem(IState states, string propertyPath, ArrayIndices arrayIndices)
    {
        int index = arrayIndices[0];
        foreach (GameObject item in Items)
        {
            Destroy(item);
        }
        Items.Clear();
        InstantiateItems();
      
    }

    public void OnPointerDown(PointerEventData eventData)//Нажимание на труп 
    {
    
        LootBox.gameObject.GetComponent<UIWindow>().Show(); // Открывает окно лута 

    }

    private void InstantiateItems()
    {

        for (int i = 0; i < state.IdItem.Length; i++)
        {
            if (state.IdItem[i] != 0)
            {


                GameObject Item = Instantiate(LootItem, ContentBox.transform);//Создание предмета для лута

                Item.GetComponent<Image>().sprite = itemDatabase.LookIDItem(state.IdItem[i]).GetIconItem;// Устанавливаем иконки

                Item.GetComponent<ItemDrag>().SetId(state.IdItem[i]);// Устанавливаем айди на предмет

                Item.GetComponent<ItemDrag>().SetEntity(_entityDeathPlayer); //Сущность трупа игрока(Владелец:Сервер)

                Item.GetComponent<ItemDrag>().SetEntityPlayer(_entityPlayers);//Сущность игрока который забирает предмет
                Items.Add(Item);
            }
        }




    }
    private void FindEntityPlayer()//Метод поиска сущности игрока, владельца.
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player"); //Находит всех игроков 
        foreach (GameObject player in Players)
        {
            if (player.GetComponent<BoltEntity>().IsOwner)
            {
                _Player = player;
                _entityPlayers = _Player.GetComponent<BoltEntity>();
            }
        }
        

    }

}
