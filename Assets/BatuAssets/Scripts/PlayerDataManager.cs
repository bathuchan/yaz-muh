using System;
using System.Collections.Generic;
using Unity.Netcode;

using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public Dictionary<ulong, PlayerData> playerDataDict = new();

    public NetworkVariable<PlayerDataDictionary> syncedPlayerData = new(writePerm: NetworkVariableWritePermission.Server);

    public static event Action<float, float> OnLocalHealthChanged;
    public static event Action<float, float> OnLocalShieldChanged;

    public static event Action<ulong, float, float> OnAnyHealthChanged;
    public static event Action<ulong, float, float> OnAnyShieldChanged;

    public static event Action<ulong, string> OnAnyNameChanged;

    


    //private void Update()
    //{
    //    if (!IsClient /*|| !NetworkManager.Singleton.LocalClientId.HasValue*/)
    //        return;

    //    ulong localId = NetworkManager.Singleton.LocalClientId;

    //    if (TryGetPlayerData(localId, out PlayerData data))
    //    {
    //        float currentHealth = data.GetStat(PlayerData.PlayerStatType.CurrentHealth);
    //        float maxHealth = data.GetStat(PlayerData.PlayerStatType.MaxHealth);

    //        float currentShield = data.GetStat(PlayerData.PlayerStatType.CurrentShield);
    //        float maxShield = data.GetStat(PlayerData.PlayerStatType.MaxShield);

    //        if (!Mathf.Approximately(currentHealth, lastKnownHealth))
    //        {
    //            lastKnownHealth = currentHealth;
    //            OnLocalHealthChanged?.Invoke(currentHealth, maxHealth);
    //        }

    //        if (!Mathf.Approximately(currentShield, lastKnownShield))
    //        {
    //            lastKnownShield = currentShield;
    //            OnLocalShieldChanged?.Invoke(currentShield, maxShield);
    //        }
    //    }
    //}


    public override void OnNetworkSpawn()
    {
        if (Instance == null)
            Instance = this;


        syncedPlayerData.OnValueChanged += OnSyncedPlayerDataChanged;


        if (IsServer)
        {
            Debug.Log("PlayerDataManager spawned on server.");
            syncedPlayerData.Value = new PlayerDataDictionary { dict = new Dictionary<ulong, PlayerData>() };
            //NetworkManager.OnConnectionEvent += OnClientConnected;
        }

        
    }

    private void OnSyncedPlayerDataChanged(PlayerDataDictionary previous, PlayerDataDictionary current)
    {
        foreach (var kvp in current.dict)
        {
            ulong playerId = kvp.Key;
            var newData = kvp.Value;

            previous.dict.TryGetValue(playerId, out var oldData);

            float oldHealth = oldData.GetStat(PlayerData.PlayerStatType.CurrentHealth);
            float newHealth = newData.GetStat(PlayerData.PlayerStatType.CurrentHealth);
            float maxHealth = newData.GetStat(PlayerData.PlayerStatType.MaxHealth);

            if (!Mathf.Approximately(oldHealth, newHealth))
            {
                OnAnyHealthChanged?.Invoke(playerId, newHealth, maxHealth);
            }

            float oldShield = oldData.GetStat(PlayerData.PlayerStatType.CurrentShield);
            float newShield = newData.GetStat(PlayerData.PlayerStatType.CurrentShield);
            float maxShield = newData.GetStat(PlayerData.PlayerStatType.MaxShield);

            if (!Mathf.Approximately(oldShield, newShield))
            {
                OnAnyShieldChanged?.Invoke(playerId, newShield, maxShield);
            }

            string oldName = oldData.userName;
            string newName = newData.userName;

            if (!string.Equals(oldName, newName))
            {
                OnAnyNameChanged?.Invoke(playerId, newName);
            }

            //Optionally: if this is the local player, raise the local - only versions too
            if (playerId == NetworkManager.Singleton.LocalClientId)
            {
                OnLocalHealthChanged?.Invoke(newHealth, maxHealth);
                OnLocalShieldChanged?.Invoke(newShield, maxShield);
            }
        }
    }


    //private void OnClientConnected(NetworkManager networkManager, ConnectionEventData connectionEventData)
    //{
    //    if (!playerDataDict.ContainsKey(connectionEventData.ClientId))
    //    {
    //        Debug.Log($"Client {connectionEventData.ClientId} connected. Creating base player data.");
    //        CreateBasePlayerData(connectionEventData.ClientId);
    //    }

    //}

    public override void OnNetworkDespawn()
    {

        syncedPlayerData.OnValueChanged -= OnSyncedPlayerDataChanged;


        if (IsServer)
        {
            //NetworkManager.OnConnectionEvent -= OnClientConnected;
        }
        
    }


    //public void CreateBasePlayerData(ulong netID)
    //{
    //    if (!IsServer) return;

    //    var playerData = new PlayerData(netID,/*somehow add name here*/);
    //    playerDataDict[netID] = playerData;
    //    syncedPlayerData.Value.dict[netID] = playerData;

    //    syncedPlayerData.SetDirty(true); // Force sync
    //}

    public void UpdatePlayerData(ulong netID, PlayerData newData)
    {
        if (!IsServer) return;

        if (playerDataDict.ContainsKey(netID))
        {
            playerDataDict[netID] = newData;
            syncedPlayerData.Value.dict[netID] = newData;

            syncedPlayerData.SetDirty(true); // Force sync
        }
    }

    /// <summary>
    /// Updates a single stat value inside PlayerData by its stat type.
    /// </summary>
    public void SetStatValue(ulong netID, PlayerData.PlayerStatType stat, float value)
    {
        if (!IsServer) return;

        if (playerDataDict.TryGetValue(netID, out PlayerData data))
        {
            data.SetStat(stat, value);
            UpdatePlayerData(netID, data);
        }
    }


    public bool TryGetStatValue(ulong netID, PlayerData.PlayerStatType stat, out float value)
    {
        value = 0f;

        if (!IsServer) return false;

        if (playerDataDict.TryGetValue(netID, out PlayerData data))
        {
            value = data.GetStat(stat);
            return true;
        }

        return false;
    }


    public bool TryGetPlayerData(ulong netID, out PlayerData data)
    {
        return syncedPlayerData.Value.dict.TryGetValue(netID, out data);
    }

    public void SetPlayerName(ulong playerId, string name)
    {
        if (!playerDataDict.ContainsKey(playerId)) return;

        playerDataDict[playerId].SetName(name);

        syncedPlayerData.Value.dict[playerId] = playerDataDict[playerId];
        syncedPlayerData.SetDirty(true);

        OnAnyNameChanged?.Invoke(playerId, name);
    }

   

    [ServerRpc(RequireOwnership = false)]
    public void RequestFullPlayerDataSyncServerRpc(ServerRpcParams rpcParams = default)
    {
        // When a new client joins, send all current player stats
        foreach (var kvp in playerDataDict)
        {
            ulong playerId = kvp.Key;
            PlayerData data = kvp.Value;

            //data.SetName(PlayerInfo.Instance.name);//this instance not set on server build(?)
            //Debug.Log("[SERVER] Name set to :" + PlayerInfo.Instance.name);

            float currentHealth = data.GetStat(PlayerData.PlayerStatType.CurrentHealth);
            float maxHealth = data.GetStat(PlayerData.PlayerStatType.MaxHealth);

            float currentShield = data.GetStat(PlayerData.PlayerStatType.CurrentShield);
            float maxShield = data.GetStat(PlayerData.PlayerStatType.MaxShield);

            SendHealthUpdateClientRpc(playerId, currentHealth, maxHealth, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
                }
            });

            SendShieldUpdateClientRpc(playerId, currentShield, maxShield, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
                }
            });

            //BroadcastPlayerNameClientRpc(playerId, userName, new ClientRpcParams
            //{
            //    Send = new ClientRpcSendParams
            //    {
            //        TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
            //    }
            //});

        }
    }

    [ClientRpc]
    private void SendHealthUpdateClientRpc(ulong playerId, float current, float max, ClientRpcParams rpcParams = default)
    {
        OnAnyHealthChanged?.Invoke(playerId, current, max);
    }

    [ClientRpc]
    private void SendShieldUpdateClientRpc(ulong playerId, float current, float max, ClientRpcParams rpcParams = default)
    {
        OnAnyShieldChanged?.Invoke(playerId, current, max);
    }

    [ClientRpc]
    public void BroadcastPlayerNameClientRpc(ulong playerId, string playerName, ClientRpcParams rpcParams = default)
    {
        OnAnyNameChanged?.Invoke(playerId, playerName);
    }


}
