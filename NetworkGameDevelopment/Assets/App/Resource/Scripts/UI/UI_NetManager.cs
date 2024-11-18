using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class UI_NetManager : NetworkBehaviour
{
    [SerializeField] private Button _serverButt, _clientButt, _hostButt, _startButt;

    [SerializeField] private GameObject _connectionButtons;
    

    [SerializeField] private SpawnController _mySpawnController;

    void Start()
    {
        _hostButt.onClick.AddListener(HostClick);
        _clientButt.onClick.AddListener(ClientClick);
        _serverButt.onClick.AddListener(ServerClick);
        _startButt.onClick.AddListener(StartClick);
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
            _startButt.gameObject.SetActive(false);
        }

    }
}
