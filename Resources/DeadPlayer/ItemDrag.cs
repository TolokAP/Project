using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Bolt;

public class ItemDrag : GlobalEventListener, IBeginDragHandler, IDragHandler, IEndDragHandler
{   private Transform startParent;
    [SerializeField]
    private int _id;

    private BoltEntity _entityDeadPlayer; // ссылка на сущность трупа
    private BoltEntity _entityPlayerWhoLootItem;//ссылк на сущность персонажа кто лутает


    private void Start()
    {
        startParent = transform.parent;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;

    }

    public void SetParametr(int id, BoltEntity entity, BoltEntity entityDeadPlayer)// установка параметров предмета при создании лута
    {
        _id = id;

        _entityDeadPlayer = entityDeadPlayer;

        _entityPlayerWhoLootItem = entity;
    }
    public int GetId()
    {
        return _id;
    }
   
    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
        transform.SetParent(startParent.transform, true);
        
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Loot"))
        {
            IDLootItem.Post(GlobalTargets.OnlyServer,_entityDeadPlayer,_id,_entityPlayerWhoLootItem);
           
            Debug.Log("Перемещено на лут");
            GameObject.Destroy(gameObject);
        }
    }

  
}
