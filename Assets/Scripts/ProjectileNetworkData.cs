using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileNetworkData : INetworkSerializable
{
    public int projectileId;
    public SpawnInfo spawnInfo;

    // Default constructor (required by Netcode)
    public ProjectileNetworkData()
    {
        projectileId = 0; // Assign default values
        spawnInfo = new SpawnInfo
        {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            direction = Vector3.zero,
            playerVelocity = Vector3.zero
        };
    }

    // Custom constructor
    public ProjectileNetworkData(int projectileId, Vector3 position, Quaternion rotation, Vector3 direction, Vector3 playerVelocity)
    {
        this.projectileId = projectileId;
        spawnInfo.position = position;
        spawnInfo.rotation = rotation;
        spawnInfo.direction = direction;
        spawnInfo.playerVelocity = playerVelocity;
    }

    // Default serializer (required by spawned objects over server)
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref projectileId);
        serializer.SerializeValue(ref spawnInfo.position);
        serializer.SerializeValue(ref spawnInfo.rotation);
        serializer.SerializeValue(ref spawnInfo.direction);
        serializer.SerializeValue(ref spawnInfo.playerVelocity);
    }
}
