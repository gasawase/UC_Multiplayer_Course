using System;
using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

public class ExplosionScript : NetworkBehaviour
{
    [SerializeField] private int _damage;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Player hit!");
            other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void EndAnimRpc()
    {
        NetworkObject.Despawn();
    }
}
