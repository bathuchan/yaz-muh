using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "Game/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public int projectileId;  // Unique ID for network reference ( 0 = null DONT USE)
    public GameObject prefab;
    public float baseDamage;
    public ElementType elementType;
    public float baseCriticalChange;
    public float speed;
    public float cooldown;
    public float duration;
    public float range;
    public string description;
    
}
public enum ElementType { Fire, Water, Nature }

public struct SpawnInfo: INetworkSerializable
{
    public int projectileId;
    
    public Vector3 direction;


    

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref projectileId);
        
        serializer.SerializeValue(ref direction); 
        
    }
}
