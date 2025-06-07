using System;
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

    //public string serverIp = "192.168.x.x";

    public int port;
    public string address;


    private void Awake()
    {
        string[] args = Environment.GetCommandLineArgs();

        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", ushort.Parse(args[0]));
            NetworkManager.Singleton.StartServer();

        });
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();

        });
        clientBtn.onClick.AddListener(() =>
        {

            NetworkManager.Singleton.StartClient();


        });

    }

}
