using System;
using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

    public class NetworkedPlayerData : NetworkBehaviour
    {
        public NetworkList<PlayerInfoData> _allConnectedPlayers; // current connected players in game
        private int _players = -1; // to account for host?
        private ulong _serverLocalID;
        
        private Color[] _PlayerColors = new Color[] { Color.red, Color.green, Color.blue };

        private void Awake()
        {
            // Avoid mem leaks by initializing the network list here
            _allConnectedPlayers = new NetworkList<PlayerInfoData>(readPerm: NetworkVariableReadPermission.Everyone);
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvents;
                _serverLocalID = NetworkManager.ServerClientId;
            }

        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvents;
            }
            
            base.OnNetworkDespawn();
        }



        private void OnConnectionEvents(NetworkManager netManager, ConnectionEventData eventData)
        {
            if (eventData.EventType == ConnectionEvent.ClientConnected)
            {
                // when client connects create data
                Debug.Log(eventData.ClientId);
                CreateNewClientData(eventData.ClientId);
                
            }
            if (eventData.EventType == ConnectionEvent.ClientDisconnected)
            {
                _players--;
            }
        }



        private void CreateNewClientData(ulong clientID)
        {
            // creating new player info
            PlayerInfoData playerInfoData = new PlayerInfoData(clientID);
            // check to see if server  matches parameter clientID
            
            // TODO: add or modify name
            
            if (_serverLocalID == clientID)
            {
                // if we are the host, assume we are always ready
                playerInfoData._isPlayerReady = true;
            }
            else
            {
                // clients are set to false
                playerInfoData._isPlayerReady = false;
            }
            
            _players++;
            
            playerInfoData._colorId = _PlayerColors[_players];
            
            
            _allConnectedPlayers.Add(playerInfoData); // adding the playerinfodata to the network list of player info data
        }

        public void RemovePlayerData(PlayerInfoData playerData)
        {
            _allConnectedPlayers.Remove(playerData);
        }

        public PlayerInfoData FindPlayerInfoData(ulong clientID)
        {
            return _allConnectedPlayers[FindPlayerIndex(clientID)];
        }

        private int FindPlayerIndex(ulong clientID)
        {
            int myMatch = -1;

            for (int i = 0; i < _allConnectedPlayers.Count; i++)
            {
                if (clientID == _allConnectedPlayers[i]._clientId)
                {
                    myMatch = i;
                }
            }
            
            return myMatch;
        }

        public void UpdateReadyClient(ulong clientID, bool _isReady)
        {
            int indx = FindPlayerIndex(clientID);

            if (indx == -1) { return; }
            
            // Grab info, change it, and pass it back to the NetworkList
            PlayerInfoData playerInfo = new PlayerInfoData();
            // copy data
            playerInfo = _allConnectedPlayers[indx];
            // change status
            playerInfo._isPlayerReady = _isReady;
            // update new status in list
            _allConnectedPlayers[indx] = playerInfo;
        }
    }

