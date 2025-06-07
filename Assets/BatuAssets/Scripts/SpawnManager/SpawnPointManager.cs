using UnityEngine;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance { get; private set; }

    [SerializeField] private List<Transform> spawnPoints;

    private int lastUsedIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Transform GetNextSpawnPoint()
    {
        if (spawnPoints.Count == 0) return null;

        lastUsedIndex = (lastUsedIndex + 1) % spawnPoints.Count;
        return spawnPoints[lastUsedIndex];
    }

    // Optional: call this to reset when scene loads or players leave
    public void ResetSpawnIndex()
    {
        lastUsedIndex = -1;
    }
}
