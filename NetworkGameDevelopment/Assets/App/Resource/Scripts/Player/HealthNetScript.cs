using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace App.Resource.Scripts.Player
{
    public class HealthNetScript : NetworkBehaviour
    {
        [SerializeField] private float _startingHealth = 100f;
        [SerializeField] private float _coolDown = 1.5f;
        [SerializeField] private bool _canDamage = true;
        [SerializeField] private NetworkVariable<float> _Health = new NetworkVariable<float>(100);
        [SerializeField] public Slider _healthSlider;

        private GameScript _gameScript;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // initialize health
            _Health.Value = _startingHealth;
            
            // Tap into the on value changes
            _Health.OnValueChanged += UpdateHealth;
        }

        private void UpdateHealth(float previousvalue, float newvalue)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = newvalue / _startingHealth;
            }

            if (IsOwner)
            {
                if (newvalue < 0f)
                {
                    // talk to the game script
                    FindObjectOfType<GameScript>().PlayerDeathRpc();
                    HasDiedRpc();
                }
            }
            // TODO: Sync network data here
        }

        [Rpc(SendTo.Server)]
        public void HasDiedRpc()
        {
            NetworkObject.Despawn();
        }
        
        // Because we want to have other things damage the player, we want ownership false
        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void DamageObjRpc(float dmg)
        {
            if (!_canDamage) return;
            _Health.Value -= dmg;
            StartCoroutine(nameof(DamageCooldown));
            Debug.Log($"Damage received : {dmg}");
        }

        private IEnumerable DamageCooldown()
        {
            _canDamage = false;
            yield return new WaitForSeconds(_coolDown);
            _canDamage = true;
        }
    }
}

