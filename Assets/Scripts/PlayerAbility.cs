using Unity.Netcode;
using UnityEngine;

public class PlayerAbility : NetworkBehaviour
{
    [SerializeField] private int projectileId; // Set this ID in Inspector or use keys "1-2" to change projectile type
    [SerializeField] private Transform firePoint;

    private PlayerNetwork playerNetwork;

    private void Start()
    {
        playerNetwork = GetComponent<PlayerNetwork>();
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
                position = firePoint.position,
                direction = firePoint.forward,
                playerVelocity = playerNetwork.playerRb.velocity
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
        

        //ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        //if (projectileData == null)
        //{
        //    Debug.LogError("[CLIENT] Invalid projectile ID!");
        //    return;
        //}

        //GameObject projectileInstance = Instantiate(
        //    projectileData.prefab,
        //    spawnInfo.position,
        //    Quaternion.identity
        //);

        //Debug.Log("[CLIENT] Instantiated projectile successfully!");

        //Projectile projectile = projectileInstance.GetComponent<Projectile>();
        //if (projectile == null)
        //{
        //    Debug.LogError("[CLIENT] Spawned object does not have a Projectile component!");
        //    return;
        //}

        //projectile.Initialize(spawnInfo);

        SpawnProjectileClientRpc(spawnInfo);
    }


    [ClientRpc]
    private void SpawnProjectileClientRpc(SpawnInfo spawnInfo)
    {
        Debug.Log($"[CLIENT] Received SpawnProjectileClientRpc! Projectile ID: {spawnInfo.projectileId}");

        ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        if (projectileData == null)
        {
            Debug.LogError("[CLIENT] Invalid projectile ID!");
            return;
        }

        GameObject projectileInstance = Instantiate(
            projectileData.prefab,
            spawnInfo.position,
            Quaternion.identity
        );

        Debug.Log("[CLIENT] Instantiated projectile successfully!");

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("[CLIENT] Spawned object does not have a Projectile component!");
            return;
        }

        projectile.Initialize(spawnInfo);
    }

}
