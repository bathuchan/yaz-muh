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

    [SerializeField] private BuildType currentBuildType;
    [SerializeField] private GameObject uiButtonsParent;
   enum BuildType 
    {
        Server,
        Host, 
        Client,
        Null
    }

    UnityTransport unityTransport;
    private void Awake()
    {
        //UnityTransport unityTransport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
         unityTransport = networkManager.NetworkConfig.NetworkTransport as UnityTransport;

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

    private void Start()
    {
        switch (currentBuildType)
        {
            case BuildType.Client:
                // code block
                ApplyConnectionSettings(unityTransport);
                NetworkManager.Singleton.StartClient();
                uiButtonsParent.SetActive(false);



                break;
            case BuildType.Server:
                // code block
                ApplyConnectionSettings(unityTransport);
                NetworkManager.Singleton.StartServer();
                uiButtonsParent.SetActive(false);

                break;

            case BuildType.Host:
                ApplyConnectionSettings(unityTransport);
                NetworkManager.Singleton.StartHost();
                uiButtonsParent.SetActive(false);

                break;
            default:
                Debug.LogWarning("Build type not declared!");
                uiButtonsParent.SetActive(true);

                break;
        }

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