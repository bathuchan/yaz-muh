using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Rigidbody rb;
    private SpawnInfo spawnInfo;
    private ProjectileData projectileData;

    public void Initialize(SpawnInfo spawnInfo, Collider shooterCollider)
    {
        this.spawnInfo = spawnInfo;
        this.projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);

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
        rb.velocity = (spawnInfo.direction.normalized * projectileData.speed) + spawnInfo.playerVelocity;
    }

    private void Update()
    {
        if (Vector3.Distance(spawnInfo.position, transform.position) >= projectileData.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[PROJECTILE] Hit player: {other.name}");
        }

        Destroy(gameObject);
    }
}
