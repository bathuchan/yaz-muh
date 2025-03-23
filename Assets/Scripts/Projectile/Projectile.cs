using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    private SpawnInfo spawnInfo;
    private ProjectileData projectileData;

    public void Initialize(SpawnInfo spawnInfo)
    {
        this.spawnInfo = spawnInfo;
        this.projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);//FOR PREVENTING CHEATS

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

        // Apply initial velocity
        rb.isKinematic = false;
        rb.velocity = (spawnInfo.direction.normalized * projectileData.speed) + spawnInfo.playerVelocity;
    }

    private void Update()
    {
        // Destroy if out of range
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
