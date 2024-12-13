using System;
using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Obj;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.Obj
{
    // TODO: add a pickup object pool
    public class AmmoPickup : NetworkBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (!IsServer) return;
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<BulletSpawner>()._ammoAmount.Value++;
                
                Destroy(gameObject);
            }
        }
    }
}

