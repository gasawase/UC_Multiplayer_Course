using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.Obj
{
    public class BulletSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _projectilePrefab;
        [SerializeField] private NetworkVariable<int> _ammoAmount;
        [SerializeField] private Transform _startingPoint;

        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void FireProjectileRpc(RpcParams rpcParams = default)
        {
            if (_ammoAmount.Value > 0)
            {
                // Spawn our bullet at your starting point position and use spawn with ownership so we can "own" the projectile
                NetworkObject newProjectile = NetworkManager.Instantiate(_projectilePrefab, _startingPoint.position,
                    _startingPoint.rotation);
                newProjectile.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
                
                _ammoAmount.Value--;
            }
        }
    }
}
