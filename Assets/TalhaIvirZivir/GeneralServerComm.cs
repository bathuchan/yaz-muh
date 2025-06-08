using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using LiteNetLib.Utils;

public enum CMD_STATUS
{
    ERR_UNKNOWN,
    ERR_PASS_SHORT,
    ERR_PASS_LONG,
    ERR_USERNAME_EXISTS,
    ERR_EMAIL_EXISTS,
    RET_SUCCESSFUL,
    CMD_REGISTER,
    CMD_LOGIN,
    RET_FAIL,

    CMD_JOIN_QUEUE,
    CMD_LEAVE_QUEUE,
    INFO_JOIN_GAME
}


public class GeneralServerComm : MonoBehaviour, INetEventListener
{
    public static GeneralServerComm Instance { get; private set; }

    public NetPeer server;

    public bool dataFlag = false;

    public int return_code = 0;

    public NetDataReader data_reader;

    public NetManager client;
    public void OnConnectionRequest(ConnectionRequest request)
    {
        Debug.Log("dsfsdfds");
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.Log("errorrr");
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        Debug.Log("Data recieved");
        int ret = reader.GetInt();
        data_reader = reader;




        return_code = ret;
        dataFlag = true;
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        Debug.Log("uncom");
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("Connected to server(General)");
        server = peer;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("dis");
    }




    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            client = new NetManager(this);
            client.Start();
            client.Connect("100.125.153.75", 9060, "SampleApp");

            client.Start(9050);
            Debug.Log("LiteNetLib NetManager started.");
            DontDestroyOnLoad(gameObject); // Optional: Keep this object alive across scene loads
        }
        else if (Instance != this)
        {
            // If another instance already exists, destroy this one
            Destroy(gameObject);
            Debug.LogWarning("Duplicate GeneralServerComm instance destroyed.");
            return;
        }

    }

    void OnApplicationQuit()
    {
        // Clean up LiteNetLib when the application quits
        client?.Stop();
        Debug.Log("LiteNetLib NetManager stopped.");
    }

    // Update is called once per frame
    void Update()
    {
        client.PollEvents();
    }

    public void sendToPeer(NetDataWriter writer)
    {
        Debug.Log("Data sent");
        server.Send(writer, DeliveryMethod.ReliableOrdered);
    }
}
