using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    //THIS IS NOT USED FOR NOW
    //THIS IS AN IDEA FOR OPTIMIZATION
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> projectilePool;

    private void Awake()
    {
        projectilePool = new Queue<GameObject>();

        // Pre-instantiate projectiles and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab);
            projectileInstance.SetActive(false); // Initially deactivate projectiles
            projectilePool.Enqueue(projectileInstance);
        }
    }

    // Get a projectile from the pool
    public GameObject GetProjectile()
    {
        if (projectilePool.Count > 0)
        {
            GameObject projectile = projectilePool.Dequeue();
            projectile.SetActive(true); // Activate the projectile
            return projectile;
        }
        else
        {
            // If no projectiles are available, you could instantiate a new one if needed
            GameObject projectileInstance = Instantiate(projectilePrefab);
            return projectileInstance;
        }
    }

    // Return a projectile back to the pool
    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false); // Deactivate the projectile
        projectilePool.Enqueue(projectile);
    }
}
