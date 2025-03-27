using Unity.Netcode;
using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerAbility : NetworkBehaviour
{
    [SerializeField] private int projectileId; // Set this ID in Inspector or use keys "1-2" to change projectile type
    [SerializeField] private Transform firePoint;

    private PlayerNetwork playerNetwork;
    private PlayerState playerState;
    Collider shooterCollider;
    private PlayerLook playerLook;
    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        playerLook= GetComponent<PlayerLook>();
    }
    private void Start()
    {
        playerNetwork = GetComponent<PlayerNetwork>();
        shooterCollider = GetComponentInChildren<Collider>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Change casted ability
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            projectileId = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            projectileId = 2;
        }

        // Cast ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (firePoint == null)
            {
                Debug.LogError("FirePoint is not assigned!");
                return;
            }

            ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(projectileId);

            if (projectileData == null)
            {
                Debug.LogError("Invalid projectile ID! Make sure it exists in the database.");
                return;
            }
            
           SpawnInfo spawnInfo = new SpawnInfo
            {
                projectileId = this.projectileId,
                
                direction = playerLook.playerModel.transform.forward
                
            };


            Debug.Log("[CLIENT] Requesting Server to spawn projectile on all clients...");
            RequestProjectileSpawnServerRpc(spawnInfo);
        }

    }

    [ServerRpc]
    private void RequestProjectileSpawnServerRpc(SpawnInfo spawnInfo)
    {
        
        Debug.Log($"[SERVER] Received request to spawn projectile {spawnInfo.projectileId}");

        //TODO:somehow need to create a projectile instance on serverside as well and check if client side projectile and this serverside projectile matchs.
        //Or only trust the serverside projectile to register collisions
        

        SpawnProjectileClientRpc(spawnInfo);
    }


    [ClientRpc]
    private void SpawnProjectileClientRpc(SpawnInfo spawnInfo)
    {
        Debug.Log($"[CLIENT] Received SpawnProjectileClientRpc! Projectile ID: {spawnInfo.projectileId}");

        ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        if (projectileData == null)
        {
            Debug.LogError("[SERVER] Invalid projectile ID!");
            return;
        }

        GameObject projectileInstance = Instantiate(
            projectileData.prefab,
            firePoint.position,
            Quaternion.identity
        );

        // Initialize projectile with shooter collider

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {
            
            projectile.Initialize(spawnInfo, firePoint.position, shooterCollider,playerNetwork,playerState);
        }
        else
        {
            Debug.LogError("[SERVER] Spawned projectile is missing the Projectile component!");
        }
    }

}
