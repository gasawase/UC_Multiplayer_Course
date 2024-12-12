using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.Obj
{
    public class TntScript : NetworkBehaviour
    {
        public float _fuseTimer;

        public NetworkObject ExplosionPrefab;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                StartCoroutine("TriggerFuse");
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Projectile" || other.gameObject.tag.Equals("Explosion"))
            {
                TriggerExplosionRpc();
            }
        }

        private IEnumerator TriggerFuse()
        {
            Debug.Log("Fuse lit!");
            yield return new WaitForSeconds(_fuseTimer);
            TriggerExplosionRpc();
        }

        [Rpc(SendTo.Server)]
        public void TriggerExplosionRpc()
        {
            NetworkObject explosive =
                NetworkManager.Instantiate(ExplosionPrefab, transform.position, transform.rotation);

            explosive.Spawn(true);
            this.NetworkObject.Despawn();
        }

    }
}
