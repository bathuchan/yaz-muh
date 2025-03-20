using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public float currentHealth, maxHealth;
    public float armor;
    public float damageMultiplier;
    public float criticalChance;
    public float criticalMultiplier;
    public float shield;
    public float shieldMultiplier;
    public float cooldownSpeedMultiplier;
    public float movementSpeed;
    public float healingMultiplier;
    public float omnivampMultiplier;

    //Constructor 
    public PlayerData(float maxHealth, float shield)
    {
        this.currentHealth = maxHealth; // Full health at start
        this.maxHealth = maxHealth;
        this.armor = 0;
        this.damageMultiplier = 1;
        this.criticalChance = 0;
        this.criticalMultiplier = 1;
        this.shield = shield;
        this.shieldMultiplier = 1;
        this.cooldownSpeedMultiplier = 1;
        this.movementSpeed = 4f;
        this.healingMultiplier = 1;
        this.omnivampMultiplier = 0;
    }

    //Syncs all fields
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref currentHealth);
        serializer.SerializeValue(ref maxHealth);
        serializer.SerializeValue(ref armor);
        serializer.SerializeValue(ref damageMultiplier);
        serializer.SerializeValue(ref criticalChance);
        serializer.SerializeValue(ref criticalMultiplier);
        serializer.SerializeValue(ref shield);
        serializer.SerializeValue(ref shieldMultiplier);
        serializer.SerializeValue(ref cooldownSpeedMultiplier);
        serializer.SerializeValue(ref movementSpeed);
        serializer.SerializeValue(ref healingMultiplier);
        serializer.SerializeValue(ref omnivampMultiplier);
    }

    //Compares all fields
    public bool Equals(PlayerData other)
    {
        return currentHealth == other.currentHealth &&
               maxHealth == other.maxHealth &&
               armor == other.armor &&
               damageMultiplier == other.damageMultiplier &&
               criticalChance == other.criticalChance &&
               criticalMultiplier == other.criticalMultiplier &&
               shield == other.shield &&
               shieldMultiplier == other.shieldMultiplier &&
               cooldownSpeedMultiplier == other.cooldownSpeedMultiplier &&
               movementSpeed == other.movementSpeed &&
               healingMultiplier == other.healingMultiplier &&
               omnivampMultiplier == other.omnivampMultiplier;
    }
}


