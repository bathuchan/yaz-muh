using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnHandler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SetSpawnPositionAndParent(OwnerClientId);
    }

    private void SetSpawnPositionAndParent(ulong clientId)
    {
        var playerObj = NetworkManager.ConnectedClients[clientId].PlayerObject;

        // Set spawn position
        Transform spawnPoint = SpawnPointManager.Instance.GetNextSpawnPoint();
        if (spawnPoint != null)
        {
            playerObj.transform.position = spawnPoint.position;
            playerObj.transform.rotation = spawnPoint.rotation;
        }

        // Parent under container
        playerObj.TrySetParent(ContainerController.Instance.GetContainer("Player"), true);
    }
}
