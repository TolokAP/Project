using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Bolt;
using Photon.Bolt.Utils;
using Photon.Bolt.Matchmaking;

namespace Player
{
    public class ArrowScripts : EntityBehaviour<IArrowArcher>
    {
       
        private NetworkId targetNetworkID;
        private BoltEntity _entityOwnerArrow;
        private BoltEntity _entytiTarget;
        private Transform targetPosition;
        private int damage;
        private float speed;
    

        public override void Attached()
        {
            var positionTarget = (TargetPosition)entity.AttachToken;
            targetNetworkID = new NetworkId(positionTarget.NetworkID);
            _entytiTarget = BoltNetwork.FindEntity(targetNetworkID);
            var netIdOwnerArrow = new NetworkId(positionTarget.NetworkIDOwner);
            _entityOwnerArrow = BoltNetwork.FindEntity(netIdOwnerArrow);
            damage = positionTarget.Damage;
            state.SetTransforms(state.Transform, transform);
            speed = 30f;
            targetPosition = _entytiTarget.gameObject.transform;

            
        }



        public override void SimulateOwner()
        {
            if (_entytiTarget.gameObject != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
            }

        }
        private IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.5f);
            BoltNetwork.Destroy(gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (entity.IsOwner)
            {
                if (other.CompareTag("Player"))
                {
                    BoltEntity _entytyTargetCollider = other.GetComponent<BoltEntity>();
                    BoltLog.Warn("Попал в коллайдер " + _entytyTargetCollider.ToString());
                    if (_entytyTargetCollider == _entytiTarget)
                    {
                        SetDamage.Post(_entytyTargetCollider, damage, _entytyTargetCollider,false,entity);
                        _entytyTargetCollider.gameObject.GetComponent<Health>().AnimationDamage(damage.ToString());

                        StartCoroutine(Destroy());
                        BoltLog.Warn("Попал в цель");

                    }
                    else
                    {
                        BoltLog.Warn("промах");
                    }

                }
            }
        }

    }
}
