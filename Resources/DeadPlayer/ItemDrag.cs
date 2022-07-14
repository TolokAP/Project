using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Bolt;

public class ItemDrag : GlobalEventListener, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
{
    public Transform startParent;
    [SerializeField]
    private int _id;
    private BoltEntity _entity;
    private BoltEntity _entityPlayer;


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

    public void SetId(int id)
    {
        _id = id;
    }
    public int GetId()
    {
        return _id;
    }
    public void SetEntity(BoltEntity entity)
    {
        _entity = entity;
    }
    public void SetEntityPlayer(BoltEntity entity)
    {
        _entityPlayer = entity;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
        transform.SetParent(startParent.transform, true);
        
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Loot"))
        {
            IDLootItem.Post(GlobalTargets.OnlyServer,_entity,_id,_entityPlayer);
           
            Debug.Log("Перемещено на лут");
            GameObject.Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    
    }
}
