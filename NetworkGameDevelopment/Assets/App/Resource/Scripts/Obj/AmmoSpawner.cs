using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;


namespace App.Resource.Scripts.Obj
{
    public class AmmoSpawner : NetworkBehaviour
    {
        public NetworkObject Ammo;

        [SerializeField] private float tickTime = 6f;
        [SerializeField] private float currentTime = 6f;

        private void FixedUpdate()
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                SpawnTntRpc();
                currentTime = Random.Range(tickTime, tickTime + 5f);
            }
        }

        [ContextMenu("SpawnTNT")]
        [Rpc(SendTo.Server)]
        public void SpawnTntRpc()
        {
            NetworkObject ammo = NetworkManager.Instantiate(Ammo, transform.position, transform.rotation);
            ammo.Spawn();
        }
    }
}