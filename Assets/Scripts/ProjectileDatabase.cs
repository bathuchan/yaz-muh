using System.Collections.Generic;
using UnityEngine;

public class ProjectileDatabase : MonoBehaviour
{
    public static ProjectileDatabase Instance { get; private set; }

    [SerializeField] private List<ProjectileData> projectileList = new List<ProjectileData>(); // Visible in Inspector
    private Dictionary<int, ProjectileData> projectileLookup = new Dictionary<int, ProjectileData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadProjectiles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadProjectiles()
    {
        projectileList.Clear(); // Clear list before loading new projectiles
        projectileLookup.Clear();

        ProjectileData[] projectiles = Resources.LoadAll<ProjectileData>("Projectiles");
        foreach (var projectile in projectiles)
        {
            if (!projectileLookup.ContainsKey(projectile.projectileId))
            {
                projectileLookup.Add(projectile.projectileId, projectile);
                projectileList.Add(projectile); // Add to serialized list
            }
        }
    }

    public ProjectileData GetProjectileData(int id)
    {
        return projectileLookup.TryGetValue(id, out var data) ? data : null;
    }
}
