using App.Resource.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.Obj
{
    public class ExplosionScript : NetworkBehaviour
    {
        public float _damage;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                Debug.Log("Player hit!");
                other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
            }
        }

        [Rpc(SendTo.Server)]
        public void EndAnimRpc()
        {
            NetworkObject.Despawn();
        }
    }
}
