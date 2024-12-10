using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class UI_NetManager : NetworkBehaviour
{
    [SerializeField] private Button _serverButt, _clientButt, _hostButt, _startButt;

    [SerializeField] private GameObject _connectionButtons, _socialPanel;
    

    [SerializeField] private SpawnController _mySpawnController;

    void Start()
    {
        _startButt.gameObject.SetActive(false);
        if (_hostButt != null) _hostButt.onClick.AddListener(HostClick);
        if (_clientButt != null) _clientButt.onClick.AddListener(ClientClick);
        if (_serverButt != null) _serverButt.onClick.AddListener(ServerClick);
        if (_startButt != null) _startButt.onClick.AddListener(StartClick);
    }
    
    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
        _connectionButtons.SetActive(false);
    }
    
    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
        _connectionButtons.SetActive(false);
        _startButt.gameObject.SetActive(true);
    }
    
    private void ClientClick()
    {
        NetworkManager.Singleton.StartClient();
        _connectionButtons.SetActive(false);
    }

    private void StartClick()
    {
        if (IsServer)
        {
            // handle spawning
            _mySpawnController.SpawnAllPlayers();
            HideGuiRpc();
        }

    }

    [Rpc(SendTo.Everyone)]
    private void HideGuiRpc()
    {
        _socialPanel.SetActive(false);
    }
}
