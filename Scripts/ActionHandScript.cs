using DuloGames.UI;
using Photon.Bolt;
using Photon.Bolt.Utils;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionHandScript : EntityEventListener<IPlayer>
{
    [SerializeField]
    private RaycastHit _hit; //������ ���� ���������� 

    [SerializeField]
    private GameObject _LootBox; //���� ����

   
    public UIWindow _lootWindow;
    [SerializeField]
    private GameObject _ContentBox;//GridLayot ���� ��������� ������ ��������� ��� ����


    public ItemDatabase itemDatabase;//������ �� ���� ������ ���������

    [SerializeField]
    private GameObject _LootItem;//������ ������� ����

    public List<GameObject> Items = new List<GameObject>();//������ ��������� ��� ����


    public override void Attached()
    {
        if (!entity.IsOwner) return;

        _LootBox = GameObject.FindGameObjectWithTag("LootBox");
        _lootWindow = GameObject.FindGameObjectWithTag("LootBox").GetComponent<UIWindow>();// ��������� ������ �� ����� ���� ���������� ��������
        _ContentBox = _LootBox.GetComponentInChildren<GridLayoutGroup>().gameObject;








    }


    public void OnMouseDown()
    {

        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit, Mathf.Infinity, (1 << 8))) return;

        switch (_hit.collider.tag)
        {
            case "ItemInGround":

                LootItemInDeadPlayer(_hit.collider.gameObject,_hit.collider.GetComponent<DeathPlayerLoot>());


            break;
            case "DeadPlayer":





            break;







        }

    }


    private void LootItemInDeadPlayer(GameObject deadplayer,DeathPlayerLoot deathPlayer)
    {

        if (!deadplayer.TryGetComponent(out BoltEntity entity)) return;
        
            _lootWindow.Show();

            deathPlayer._isOpen = true;

            deathPlayer.PlayerWhoOpen = gameObject;

            InstantiateItems(deadplayer.GetComponent<BoltEntity>().GetState<IDeadPlayerState>().IdItem, deadplayer.GetComponent<BoltEntity>());

    }

    

   
    public void InstantiateItems(NetworkArray_Integer states,BoltEntity entityDeadPlayer)
    {

        BoltLog.Error("������ ������� " + states.Length + states[0] + "  " + states[1]);

        foreach (GameObject item in Items)
        {
            Destroy(item);
        }
        Items.Clear();

        for (int i = 0; i < states.Length; i++)
        {
            if (states[i] != 0)
            {

                GameObject Item = Instantiate(_LootItem, _ContentBox.transform);//�������� �������� ��� ����

                Item.GetComponent<Image>().sprite = itemDatabase.LookIDItem(states[i]).GetIconItem;// ������������� ������

                Item.GetComponent<ItemDrag>().SetParametr(states[i],entity,entityDeadPlayer);// ������������� ���� �� �������,�������� ����� ������(��������:������),�������� ������ ������� �������� �������

                Items.Add(Item);

            }
        }




    }





}
