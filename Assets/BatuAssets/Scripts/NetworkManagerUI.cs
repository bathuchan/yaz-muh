using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    [SerializeField] NetworkManager networkManager;

    [SerializeField] private string ipAdress="127.0.0.1";
    [SerializeField] private string portAdress="7777";

    private void Awake()
    {
        //UnityTransport unityTransport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        UnityTransport unityTransport = networkManager.NetworkConfig.NetworkTransport as UnityTransport;

        serverBtn.onClick.AddListener(() =>
        {
            ApplyConnectionSettings(unityTransport);
            NetworkManager.Singleton.StartServer();
        });

        hostBtn.onClick.AddListener(() =>
        {
            ApplyConnectionSettings(unityTransport);
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() =>
        {
            ApplyConnectionSettings(unityTransport);
            NetworkManager.Singleton.StartClient();
        });
    }

    private void ApplyConnectionSettings(UnityTransport transport)
    {
        string ip = ipAdress;
        ushort port = 7777;

        // Safe port parsing
        if (!ushort.TryParse(portAdress, out port))
        {
            Debug.LogWarning("Invalid port input. Falling back to default port 7777.");
            port = 7777;
        }

        transport.SetConnectionData(ip, port);
        Debug.Log($"Connection set to {ip}:{port}");
    }
}
