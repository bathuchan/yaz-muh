using Unity.Netcode;
using UnityEngine;
public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject playerDataManagerPrefab;
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var obj = Instantiate(playerDataManagerPrefab,this.transform);
                obj.GetComponent<NetworkObject>().Spawn();
            }
        };
    }
}
