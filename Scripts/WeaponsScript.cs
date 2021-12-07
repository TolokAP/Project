
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Player
{
    public class WeaponsScript : EntityBehaviour<IWeaponBase>, IPointerDownHandler
    {

         public CapsuleCollider colider;
         public TypeEquipment typeEquipment;
      

        public ParticleSystem shadow;
       
 
         private Material _normalMaterial;
         public Material glowMaterial;
         [SerializeField]
         private List<BoltEntity> _triggerEntity;
         [SerializeField]
         private bool _ground = false;
         [SerializeField]
         private BoltEntity _boltEntity;
         [SerializeField]
         private GameObject _lootEssenceItem;

        public override void Attached()// Три варианта размещения предмета в пространстве. Левая рука. Правая рука. На земле.
        {
           
            var token = (NetworkIDToken)entity.AttachToken;
            if (entity.IsOwner)
            {
                state.ID = token.ID;
             
            }
            switch (token.Hand)
            {
                case (int)WeaponHandPosition.None:

                    PositionInWorld(token.NetworkID);
                    colider = gameObject.AddComponent<CapsuleCollider>();
                    colider.radius = 2;
                    colider.isTrigger = true;
                    _ground = true;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    Instantiate(_lootEssenceItem,gameObject.transform);//создаем дым у обьекта на земле


                    break;
                case (int)WeaponHandPosition.Left:

                    PositionInHand("HandL", token.NetworkID);
                   
                    break;
                case (int)WeaponHandPosition.Right:
                    PositionInHand("HandR", token.NetworkID);
                  
                    break;
              

            }
            _normalMaterial = GetComponent<Renderer>().material;
           
           
           

        }

        public void OnTriggerEnter(Collider other)//Добавление сущностей в List. Установка материала подсветки предметов. Включение курсора.
        {
            BoltLog.Warn("Имя обьекта в коллайдере" + other.name + other.TryGetComponent<BoltEntity>(out BoltEntity dsd));
            if (other.CompareTag("Player") && _ground)
            {
                _boltEntity = other.gameObject.GetComponent<BoltEntity>();
                _triggerEntity.Add(_boltEntity);

                if (_boltEntity.IsOwner)
                {
                    gameObject.GetComponent<Renderer>().material = glowMaterial;
                 
                }
            }

        }

        public void Hit()
        {
            if (typeEquipment == TypeEquipment.weapon)
            {
                BoltLog.Warn("Работает из метода WeaponScript");

                shadow.Play();
            }
      


        }

        public void OnTriggerExit(Collider other)//Удаление сущностей из List. Установка нормального материала предмета.
        {
            _boltEntity = other.GetComponent<BoltEntity>();
            if (other.CompareTag("Player")&&_ground&&_boltEntity.IsOwner)
            {
                _triggerEntity.Remove(_boltEntity);
                gameObject.GetComponent<Renderer>().material = _normalMaterial;
              
            }

        }

        private void PositionInWorld(ulong networkID)// Определение позиции в мире после того как выкинули предмет
        {
            NetworkId network = new NetworkId(networkID);
            var _entity = BoltNetwork.FindEntity(network);
            gameObject.transform.position = _entity.gameObject.transform.position;
            if (typeEquipment == TypeEquipment.weapon||typeEquipment==TypeEquipment.twoHandWeapon)
            {
                gameObject.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            }
            else
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }


        }
        private void PositionInHand(string hand,ulong networkID)// Определение позиции оружия левая\правая рука
        {
            NetworkId network = new NetworkId(networkID);
            var _entity = BoltNetwork.FindEntity(network);
            gameObject.transform.parent = _entity.gameObject.FindChildGameObjectWithTag(hand).transform;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.Euler(90f, 0, 0);



        }

      
      

        public void OnPointerDown(PointerEventData eventData)// Событие поднятие предмета с земли в инвентарь. Событие отправляется на сервер.
        {
                    
            
            BoltLog.Warn("Работает нажатие на обьект" + eventData.pointerCurrentRaycast.gameObject.GetComponent<BoltEntity>().GetState<IWeaponBase>().ID.ToString());
            if (_triggerEntity.Count > 0)//Проверка на количество сущностей в зоне подьема предмета.
            {
                foreach (BoltEntity boltEntity in _triggerEntity)
                {
                    if (boltEntity.IsOwner)
                    {
                        var pick = PickUpItem.Create(GlobalTargets.OnlyServer);
                        pick.NetworkID = entity.NetworkId;
                        pick.ID = eventData.pointerCurrentRaycast.gameObject.GetComponent<BoltEntity>().GetState<IWeaponBase>().ID;//Получение ID предмета который поднимает игрок.
                        pick.Entity = boltEntity;
                        boltEntity.GetState<IPlayer>().PickUP();//Включение анимации подьема
                        pick.Send();
                    }
                }
              
              
            }
        }

    }
}
  