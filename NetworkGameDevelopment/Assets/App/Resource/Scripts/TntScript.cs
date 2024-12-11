using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TntScript : NetworkBehaviour
{
    [SerializeField] private int _fuseTimer;

    [SerializeField] private GameObject _explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
