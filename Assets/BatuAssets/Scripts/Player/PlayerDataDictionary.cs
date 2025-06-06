using System;

using System.Collections.Generic;
using Unity.Netcode;

[Serializable]
public struct PlayerDataDictionary : INetworkSerializable
{
    public Dictionary<ulong, PlayerData> dict;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int count = dict?.Count ?? 0;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            dict = new Dictionary<ulong, PlayerData>(count);
            for (int i = 0; i < count; i++)
            {
                ulong key = 0;
                PlayerData value = new();
                serializer.SerializeValue(ref key);
                value.NetworkSerialize(serializer);
                dict[key] = value;
            }
        }
        else
        {
            foreach (var pair in dict)
            {
                var key = pair.Key;
                var value = pair.Value;
                serializer.SerializeValue(ref key);
                value.NetworkSerialize(serializer);
            }
        }
    }
}
