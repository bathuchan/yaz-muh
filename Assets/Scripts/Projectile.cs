using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Rigidbody rb;
    public ProjectileData projectileData;
    public ProjectileNetworkData projectileNetworkData;
    public NetworkObject networkObject;

    private int projectileId;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        networkObject = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        if (projectileData == null || projectileNetworkData == null)
        {
            Debug.LogError("[SERVER] Projectile data is missing!");
            return;
        }

        Debug.Log("[SERVER] Projectile spawned successfully!");

        rb.isKinematic = false;
        rb.velocity = (projectileNetworkData.spawnInfo.direction.normalized * projectileData.speed)
                      + projectileNetworkData.spawnInfo.playerVelocity;
    }

    //Required data initialization
    public void Initialize(ProjectileNetworkData networkData)
    {
        projectileId = networkData.projectileId;
        projectileData = ProjectileDatabase.Instance.GetProjectileData(projectileId);
        projectileNetworkData = networkData;

        if (projectileData == null)
        {
            Debug.LogError($"[ERROR] ProjectileData with ID {projectileId} not found!");
            return;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        //Range checking
        if (Vector3.Distance(projectileNetworkData.spawnInfo.position, transform.position) >= projectileData.range)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsOwner)
            {
                Debug.Log("[Projectile] Other player hit");
                Despawn();
            }
            else
            {
                Debug.Log("[Projectile] Player hit itself(?)");
                return;
            }
        }
        Despawn();
    }

    private void Despawn()
    {
        
        if (IsOwner)
        {
            networkObject.Despawn(true);
            Destroy(gameObject);
        }
    }
}