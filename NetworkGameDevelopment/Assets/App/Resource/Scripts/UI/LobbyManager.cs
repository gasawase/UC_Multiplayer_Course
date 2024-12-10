using System;
using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Player;
using Resource.Scripts.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LobbyManager : NetworkBehaviour
{
    [FormerlySerializedAs("_Start")] [SerializeField] private Button _startButt;
    [SerializeField] private Button _leaveButt, _readyButt;
    [SerializeField] private GameObject _panelPrefab; // the prefab we place inside the contents
    [SerializeField] private GameObject _contentGO; // Where we are spawning the panelPrefabs
    [SerializeField] private TMP_Text _rdyText; // update status to the user
    [SerializeField] private TMP_Text _clientIDText;
    [SerializeField] private TMP_Text _chatMessagePanel;
    [SerializeField] private TMP_InputField _chatInputField;
    [SerializeField] private Button _sendButton;

    /// list of network players

    [SerializeField] private NetworkedPlayerData _networkPlayers;
    
    
    private List<GameObject> _PlayerPanels = new List<GameObject>(); // make a new list of the player panels

    private ulong _myLocalClientID;
    private ulong _localClientID;

    private bool isReady = false;

    private void Start()
    {
        _myLocalClientID = NetworkManager.ServerClientId;
        _chatMessagePanel.text = "";

        if (IsServer)
        {
            // inform server and hide ready button
            _rdyText.text = "waiting for players";
            _readyButt.gameObject.SetActive(false);
        }
        else
        {
            // client
            _rdyText.text = "Not ready";
            _readyButt.gameObject.SetActive(true);
        }
        _networkPlayers._allConnectedPlayers.OnListChanged += NetPlayersChanged;
        
        _leaveButt.onClick.AddListener(LeaveButtClicked);
        _readyButt.onClick.AddListener(ClientReadyButtToggle);
        _sendButton.onClick.AddListener(SendButtonClicked);
        Debug.Log("listeners added");
    }

    private void SendButtonClicked()
    {
        if(!IsClient && !IsHost) {return;}
        Debug.Log($"Sending button clicked: {_chatInputField.text}");
        SendChatMessageRpc(_chatInputField.text);
        _chatInputField.text = "";
    }

    private void ChatMessageReceived(ulong clientID, string message)
    {
        // this is called in the rpc on each client
        _chatMessagePanel.text = _chatMessagePanel.text + "\n" + clientID.ToString() + ": " + message;
        
    }


    private void ClientReadyButtToggle()
    {
        if(IsServer) {return;}
        isReady = !isReady;
        if (isReady)
        {
            _rdyText.text = "Ready";
        }
        else
        {
            _rdyText.text = "Not Ready";
        }
        
        RdyButtToggleServerRPC(isReady);
    }



    private void LeaveButtClicked()
    {
        if (!IsServer)
        {
            LeaveLobbyServerRpc();
        }
        else
        {
            foreach (PlayerInfoData playerdata in _networkPlayers._allConnectedPlayers)
            {
                if (playerdata._clientId != _myLocalClientID)
                {
                    KickUserBttn(playerdata._clientId);
                }
            }
            NetworkManager.Shutdown();
            SceneManager.LoadScene(0);
        }
    }



    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeEvent)
    {
        Debug.Log("Net players has changed event fired");
        PopulateLabels();

    }

    [ContextMenu("PopulateLabels")]
    // populate panels
    private void PopulateLabels()
    {
        // clear panels
        ClearPlayerPanels();
        
        // loop all player info from all networked players and then create new panels
        
        bool allReady = true; // used for logic on server

        foreach (PlayerInfoData playerData  in _networkPlayers._allConnectedPlayers)
        {
            // instantiate
            GameObject newPlayerPanel = Instantiate(_panelPrefab, _contentGO.transform);
            PlayerInfoPanel _playerLabel = newPlayerPanel.GetComponent<PlayerInfoPanel>();

            // subscribe to kick events on the panels
            _playerLabel.onKickClicked += KickUserBttn;

            // depending on client vs server, we are going to show/hide the kick btns
            if (IsServer && playerData._clientId != _myLocalClientID)
            {
                // ensure that we are the host and set active kick buttons that don't match the server
                _playerLabel.SetKickActivate(true);
                // while we are at it, ensure server's ready button is hidden, we assume server is always ready
                _readyButt.gameObject.SetActive(false);
            }
            else
            {
                // ensure clients don't have set kicked buttons are visible, but ready bttn is visible
                _playerLabel.SetKickActivate(false);
                // _readyButt.gameObject.SetActive(true);
            }

            // display info to the UI
            
            _playerLabel.SetPlayerName(playerData._clientId);
            _playerLabel.SetReadyStatus(playerData._isPlayerReady);
            _playerLabel.SetPlayerColor(playerData._colorId);
            _PlayerPanels.Add(newPlayerPanel);

            if (playerData._isPlayerReady == false)
            {
                allReady = false;
                
            }
        }
        // Check if everyone is ready, host should see if it's ready or not
        if (IsServer)
        {
            if (allReady)
            {
                if (_networkPlayers._allConnectedPlayers.Count > 1)
                {
                    _rdyText.text = "Ready to start";
                    _startButt.gameObject.SetActive(true);
                }
                else
                {
                    _rdyText.text = "Empty Lobby";
                    
                }
            }
            else
            {
                _startButt.gameObject.SetActive(false);
                _rdyText.text = "Waiting for other players";
            }
        }
    }

    private void KickUserBttn(ulong kickTarget)
    {
        // TODO: when the user clicks back in, there are two playerinfopanels created: one from the previous and one from the current session
        
        if (!IsServer || !IsHost) return;
            foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
            {
                if (playerData._clientId == kickTarget)
                {
                    // remove player from the list
                    _networkPlayers._allConnectedPlayers.Remove(playerData);
                    KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));
                    NetworkManager.Singleton.DisconnectClient(kickTarget);
                }
            }

    }
    
    

    private void ClearPlayerPanels()
    {
        Debug.Log("clear previous panels");

        
        foreach (GameObject panel in _PlayerPanels)
        {
            Destroy(panel);
        }
        _PlayerPanels.Clear();
        
    }


    // RPC CALLS //
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RdyButtToggleServerRPC(bool readyStatus, RpcParams rpcParams = default)
    {
        Debug.Log("From Rdy butt RPC");
        _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    private void KickedClientRpc(RpcParams rpcParams)
    {
        SceneManager.LoadScene(0);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void SendChatMessageRpc(string message, RpcParams rpcParams = default)
    {
        ChatMessageReceived(rpcParams.Receive.SenderClientId, message);
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void LeaveLobbyServerRpc(RpcParams rpcParams = default)
    {
        KickUserBttn(rpcParams.Receive.SenderClientId);
    }
    
    
}
