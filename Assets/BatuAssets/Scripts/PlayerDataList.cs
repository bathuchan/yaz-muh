using System;
using System.Collections.Generic;
using Unity.Netcode;

[Serializable]
public struct PlayerDataList : INetworkSerializable
{
    public List<PlayerData> players;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int count = players?.Count ?? 0;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            players = new List<PlayerData>(count);
            for (int i = 0; i < count; i++)
            {
                PlayerData data = new();
                data.NetworkSerialize(serializer);
                players.Add(data);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var data = players[i];
                data.NetworkSerialize(serializer);
            }
        }
    }
}
