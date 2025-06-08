using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public ulong clientID;
    public string userName;
    public float currentHealth;
    public float maxHealth;
    public float armor;
    public float damageMultiplier;
    public float criticalChance;
    public float criticalMultiplier;
    public float currentShield;
    public float maxShield;
    public float shieldMultiplier;
    public float cooldownSpeedMultiplier;
    public float movementSpeed;
    public float healingMultiplier;
    public float omnivampMultiplier;


    //Base Constructor 
    public PlayerData(ulong clientID,string userName)
    {
        this.clientID = clientID;
        this.userName = userName;
        this.currentHealth = 100f; // Full health at start
        this.maxHealth = this.currentHealth;
        this.armor = 0;
        this.damageMultiplier = 1f;
        this.criticalChance = 0.05f;
        this.criticalMultiplier = 1.5f;
        this.currentShield = 100f;
        this.maxShield = currentShield;
        this.shieldMultiplier = 0;
        this.cooldownSpeedMultiplier = 1;
        this.movementSpeed = 4f;
        this.healingMultiplier = 1;
        this.omnivampMultiplier = 0;
    }
    /// <summary>
    /// Constructs a new PlayerData with optional stat overrides. If a stat is not provided, the default is used.
    /// </summary>
    /// <param name="clientID">Unique identifier of the player for network tracking.</param>
    /// <param name="currentHealth">Optional current health value. Defaults to 100.</param>
    /// <param name="maxHealth">Optional maximum health. Defaults to current health.</param>
    /// <param name="armor">Optional armor value. Reduces incoming damage. Defaults to 0.</param>
    /// <param name="damageMultiplier">Optional damage multiplier. Defaults to 1.</param>
    /// <param name="criticalChance">Optional critical hit chance (0–1). Defaults to 0.</param>
    /// <param name="criticalMultiplier">Optional multiplier for critical damage. Defaults to 1.</param>
    /// <param name="currentShield">Optional shield value. Absorbs damage before health. Defaults to 100.</param>
    /// <param name="maxShield">Optional max shield value. Absorbs damage before health. Defaults to 100.</param>
    /// <param name="shieldMultiplier">Optional shield multiplier. Defaults to 1.</param>
    /// <param name="cooldownSpeedMultiplier">Optional cooldown speed multiplier. Defaults to 1.</param>
    /// <param name="movementSpeed">Optional movement speed. Defaults to 4.</param>
    /// <param name="healingMultiplier">Optional healing received multiplier. Defaults to 1.</param>
    /// <param name="omnivampMultiplier">Optional omnivamp (lifesteal) multiplier. Defaults to 0.</param>
    public PlayerData(
        ulong clientID,
        string? username="player",
        float? currentHealth = null,
        float? maxHealth = null,
        float? armor = null,
        float? damageMultiplier = null,
        float? criticalChance = null,
        float? criticalMultiplier = null,
        float? currentShield = null,
        float? maxShield = null,
        float? shieldMultiplier = null,
        float? cooldownSpeedMultiplier = null,
        float? movementSpeed = null,
        float? healingMultiplier = null,
        float? omnivampMultiplier = null)
        : this(clientID,username)
    {
        if (currentHealth.HasValue) this.currentHealth = currentHealth.Value;
        if (maxHealth.HasValue) this.maxHealth = maxHealth.Value;
        if (armor.HasValue) this.armor = armor.Value;
        if (damageMultiplier.HasValue) this.damageMultiplier = damageMultiplier.Value;
        if (criticalChance.HasValue) this.criticalChance = criticalChance.Value;
        if (criticalMultiplier.HasValue) this.criticalMultiplier = criticalMultiplier.Value;
        if (currentShield.HasValue) this.currentShield = currentShield.Value;
        if (maxShield.HasValue) this.maxShield = maxShield.Value;
        if (shieldMultiplier.HasValue) this.shieldMultiplier = shieldMultiplier.Value;
        if (cooldownSpeedMultiplier.HasValue) this.cooldownSpeedMultiplier = cooldownSpeedMultiplier.Value;
        if (movementSpeed.HasValue) this.movementSpeed = movementSpeed.Value;
        if (healingMultiplier.HasValue) this.healingMultiplier = healingMultiplier.Value;
        if (omnivampMultiplier.HasValue) this.omnivampMultiplier = omnivampMultiplier.Value;
    }


    public enum PlayerStatType
    {
        CurrentHealth,
        MaxHealth,
        Armor,
        DamageMultiplier,
        CriticalChance,
        CriticalMultiplier,
        CurrentShield,
        MaxShield,
        ShieldMultiplier,
        CooldownSpeedMultiplier,
        MovementSpeed,
        HealingMultiplier,
        OmnivampMultiplier
    }


    public float GetStat(PlayerStatType stat)
    {
        return stat switch
        {
            PlayerStatType.CurrentHealth => currentHealth,
            PlayerStatType.MaxHealth => maxHealth,
            PlayerStatType.Armor => armor,
            PlayerStatType.DamageMultiplier => damageMultiplier,
            PlayerStatType.CriticalChance => criticalChance,
            PlayerStatType.CriticalMultiplier => criticalMultiplier,
            PlayerStatType.CurrentShield => currentShield,
            PlayerStatType.MaxShield => maxShield,
            PlayerStatType.ShieldMultiplier => shieldMultiplier,
            PlayerStatType.CooldownSpeedMultiplier => cooldownSpeedMultiplier,
            PlayerStatType.MovementSpeed => movementSpeed,
            PlayerStatType.HealingMultiplier => healingMultiplier,
            PlayerStatType.OmnivampMultiplier => omnivampMultiplier,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }

    public string GetName()
    {
        return userName;
    }public void SetName(string userName)
    {
        this.userName = userName;
    }
    public void SetStat(PlayerStatType stat, float value)
    {
        switch (stat)
        {
            case PlayerStatType.CurrentHealth:
                currentHealth = value;
                break;
            case PlayerStatType.MaxHealth:
                maxHealth = value;
                break;
            case PlayerStatType.Armor:
                armor = value;
                break;
            case PlayerStatType.DamageMultiplier:
                damageMultiplier = value;
                break;
            case PlayerStatType.CriticalChance:
                criticalChance = value;
                break;
            case PlayerStatType.CriticalMultiplier:
                criticalMultiplier = value;
                break;
            case PlayerStatType.CurrentShield:
                currentShield = value;
                break;
            case PlayerStatType.MaxShield:
                maxShield = value;
                break;
            case PlayerStatType.ShieldMultiplier:
                shieldMultiplier = value;
                break;
            case PlayerStatType.CooldownSpeedMultiplier:
                cooldownSpeedMultiplier = value;
                break;
            case PlayerStatType.MovementSpeed:
                movementSpeed = value;
                break;
            case PlayerStatType.HealingMultiplier:
                healingMultiplier = value;
                break;
            case PlayerStatType.OmnivampMultiplier:
                omnivampMultiplier = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), stat, "Unsupported stat type.");
        }
    }


    //Syncs all fields

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref currentHealth);
        serializer.SerializeValue(ref maxHealth);
        serializer.SerializeValue(ref armor);
        serializer.SerializeValue(ref damageMultiplier);
        serializer.SerializeValue(ref criticalChance);
        serializer.SerializeValue(ref criticalMultiplier);
        serializer.SerializeValue(ref currentShield);
        serializer.SerializeValue(ref maxShield);
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
               currentShield == other.currentShield &&
               maxShield == other.maxShield &&
               shieldMultiplier == other.shieldMultiplier &&
               cooldownSpeedMultiplier == other.cooldownSpeedMultiplier &&
               movementSpeed == other.movementSpeed &&
               healingMultiplier == other.healingMultiplier &&
               omnivampMultiplier == other.omnivampMultiplier;
    }


}


