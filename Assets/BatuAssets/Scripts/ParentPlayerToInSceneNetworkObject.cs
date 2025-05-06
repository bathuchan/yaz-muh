using Unity.Netcode;

public class ParentPlayerToInSceneNetworkObject : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Server subscribes to the NetworkSceneManager.OnSceneEvent event
            NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;

            // Server player is parented under this NetworkObject
            SetPlayerParent(NetworkManager.LocalClientId);
        }
    }

    private void SetPlayerParent(ulong clientId)
    {
        if (IsSpawned && IsServer)
        {
            // As long as the client (player) is in the connected clients list
            if (NetworkManager.ConnectedClients.ContainsKey(clientId))
            {
                // Set the player as a child of this in-scene placed NetworkObject
                // We parent in local space by setting the WorldPositionStays value to false
                NetworkManager.ConnectedClients[clientId].PlayerObject.TrySetParent(ContainerController.Instance.GetContainer("Player"), true);
            }
        }
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        // OnSceneEvent is useful for many things
        switch (sceneEvent.SceneEventType)
        {
            // The SceneEventType event tells the server that a client-player has:
            // 1.) Connected and Spawned
            // 2.) Loaded all scenes that were loaded on the server at the time of connecting
            // 3.) Synchronized (instantiated and spawned) all NetworkObjects in the network session
            case SceneEventType.SynchronizeComplete:
                {
                    // As long as we aren't the server-player
                    if (sceneEvent.ClientId != NetworkManager.LocalClientId)
                    {
                        // Set the newly joined and synchronized client-player as a child of this in-scene placed NetworkObject
                        SetPlayerParent(sceneEvent.ClientId);
                    }
                    break;
                }
        }
    }
}