using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

// this script contains the functionality for the server
public class SpawnController : NetworkBehaviour
{

    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    
    [SerializeField] private NetworkVariable<int> _playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [SerializeField] private TMP_Text _playerCountText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); // perform base functions before subscribing methods

        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;

        }
        _playerCount.OnValueChanged += PlayerCountChanged;
        
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
        }

        _playerCount.OnValueChanged -= PlayerCountChanged;
    }

    private void PlayerCountChanged(int previousVal, int newVal)
    {
        UpdateCountTextClientRPC(newVal);
    }

    private void UpdateCountText(int newValue)
    {
        _playerCountText.text = $"Players: {newValue}";
    }

    private void OnConnectionEvent(NetworkManager netManager, ConnectionEventData eventData)
    {
        if (eventData.EventType == ConnectionEvent.ClientConnected)
        {
            // do something here when some client connects to the server

            _playerCount.Value++;
        }

        if (eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            _playerCount.Value--;
        }
    }

    public void SpawnAllPlayers()
    {
        if (!IsServer) return; // extra security

        int spawnNum = 0;
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            // instantiate prefab
            NetworkObject spawnedPlayerNwO = NetworkManager.Instantiate(_playerPrefab, _spawnPoints[spawnNum].position, 
                _spawnPoints[spawnNum].rotation); //where we start talking to the Network Manager, telling it what the player nw object would be

            // spawn it in a location based on spawn array
            spawnedPlayerNwO.SpawnAsPlayerObject(clientId); // actually spawn it
            // actually spawn the prefab
            spawnNum++;
        }
    }
    
    // RPC CALLS //
    [Rpc(SendTo.Everyone)]
    private void UpdateCountTextClientRPC(int newVal)
    {
        UpdateCountText(newVal);
    }
    
}
