using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts
{
    public class ProjectileObj : NetworkBehaviour
    {
        [SerializeField] private float _speed = 40f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _destructTime = 5f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            GetComponent<Rigidbody>().velocity = transform.forward * _speed;
            StartCoroutine(AutoDestruct());

        }

        private void OnCollisionEnter(Collision other)
        {
            // make sure its a player and that the bullet doesn't match the owner, meaning we can't do friendly fire.
            if (other.gameObject.tag.Equals("Player") &&
                other.gameObject.GetComponent<NetworkObject>().OwnerClientId != this.OwnerClientId)
            {
                other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
            }
        }

        private IEnumerator AutoDestruct()
        {
            yield return new WaitForSeconds(_destructTime);
            this.NetworkObject.Despawn();
        }
    }
}
