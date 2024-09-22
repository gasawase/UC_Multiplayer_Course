using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class UI_NetManager : MonoBehaviour
{
    [SerializeField] private Button _serverButt, _clientButt, _hostButt;

    void Start()
    {
        _hostButt.onClick.AddListener(HostClick);
        _clientButt.onClick.AddListener(ClientClick);
        _serverButt.onClick.AddListener(ServerClick);
    }

    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
    }
    
    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    private void ClientClick()
    {
        NetworkManager.Singleton.StartClient();
    }
}
