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

public struct SpawnInfo 
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 direction;
    public Vector3 playerVelocity;
}
