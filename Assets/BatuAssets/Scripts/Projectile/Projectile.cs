using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Rigidbody rb;
    private SpawnInfo spawnInfo;
    [SerializeField] private ProjectileData projectileData;
    PlayerNetwork playerNetwork;
    PlayerState playerState;
    public NetworkObject networkObject;
    Vector3 spawnPoint;
    int networkId;



    //public NetworkVariable<ulong> networkId = new NetworkVariable<ulong>(
    //    0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    //);


    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        

    }
    
    public void Initialize(SpawnInfo spawnInfo, Vector3 spawnPoint, Collider shooterCollider, PlayerNetwork playerNetwork, PlayerState playerState, int networkId)
    {


        this.spawnInfo = spawnInfo;
        this.spawnPoint = spawnPoint;
        this.projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        this.playerNetwork = playerNetwork;
        this.playerState = playerState;
        this.networkId = networkId;

        if (this.projectileData == null)
        {
            Debug.LogError("[PROJECTILE] Missing projectile data!");
            return;
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[PROJECTILE] Rigidbody is missing!");
            return;
        }

        // Ignore collision with the shooter
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null && shooterCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider);
        }


        gameObject.SetActive(true);
        


    }

    private void Start()
    {
        rb.isKinematic = false;
        rb.velocity = (spawnInfo.direction.normalized * projectileData.speed) /*+ playerNetwork.playerRb.velocity - playerState.externalForce*/;
    }

    private void Update()
    {
        if (Vector3.Distance(spawnPoint, transform.position) >= projectileData.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only the server checks collisions

        if (other.CompareTag("Player"))
        {
            Debug.Log($"[SERVER] Projectile hit {other.name}");

            // Apply effects or damage to player if needed
            DestroyProjectileServerRpc();
        }

        DestroyProjectileServerRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc()
    {
        // Send the projectileId to clients so they know which one to destroy
        DestroyProjectileClientRpc(networkId);

        // Destroy the projectile on the server
        Destroy(gameObject);
    }

    [ClientRpc]
    private void DestroyProjectileClientRpc(int projectileId)
    {
        // Look for the projectile with the given projectileId
        if (CurrentProjectiles.Instance.activeProjectiles.ContainsKey(projectileId))
        {
            GameObject projectileInstance = CurrentProjectiles.Instance.activeProjectiles[projectileId];
            if (projectileInstance != null)
            {
                // Destroy the projectile locally
                Destroy(projectileInstance);
                CurrentProjectiles.Instance.activeProjectiles.Remove(projectileId); // Remove it from the dictionary
            }
        }
        else
        {
            Debug.LogWarning("[CLIENT] Projectile not found with ID: " + projectileId);
        }
    }
}
