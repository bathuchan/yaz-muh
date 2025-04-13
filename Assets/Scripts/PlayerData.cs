using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public int playerHealth;
    public int playerShield;

    public PlayerData(int health, int shield)
    {
        this.playerHealth = health;
        this.playerShield = shield;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerHealth);
        serializer.SerializeValue(ref playerShield);
    }

    public bool Equals(PlayerData other)
    {
        return this.playerHealth == other.playerHealth && this.playerShield == other.playerShield;
    }
}
