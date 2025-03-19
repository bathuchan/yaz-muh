using Unity.Netcode;
using UnityEngine;

public class PlayerAbility : NetworkBehaviour
{
    [SerializeField] private int projectileId; // Set this ID in Inspector ore use keys "1-2" to chance cast projectile)
    [SerializeField] private Transform firePoint;

    PlayerNetwork playerNetwork;

    private void Start()
    {
        playerNetwork = GetComponentInParent<PlayerNetwork>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        //change casted ability
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            projectileId = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            projectileId = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //projectileId=3;
        }

        //cast ability
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

            // Create NetworkData to send
            ProjectileNetworkData projectileNetworkData = new ProjectileNetworkData(
                projectileId,
                firePoint.position,
                firePoint.rotation,
                firePoint.forward,
                playerNetwork.playerRb.velocity
            );

            Debug.Log($"[CLIENT] Requesting server to spawn projectile...");
            SpawnProjectileServerRpc(projectileNetworkData);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnProjectileServerRpc(ProjectileNetworkData networkData, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"[SERVER] Received projectile spawn request from Client {rpcParams.Receive.SenderClientId}");

        //get projectile data with set id
        ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(networkData.projectileId);
        if (projectileData == null)
        {
            Debug.LogError("[SERVER] Invalid projectile ID!");
            return;
        }

        //Instantiates the ability on requester
        GameObject projectileInstance = Instantiate(
            projectileData.prefab,
            networkData.spawnInfo.position,
            networkData.spawnInfo.rotation
        );

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("[SERVER] Spawned object does not have a Projectile component!");
            return;
        }

        //sets required var data on instantiated object(should find a better way to stop slight pauses on start)
        projectile.Initialize(networkData);

        NetworkObject networkObject = projectileInstance.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("[SERVER] Projectile does not have a NetworkObject component!");
            return;
        }
        //Instantiates the ability on listeners(connected clients) 
        networkObject.Spawn(true);
    }
}