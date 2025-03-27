using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Rigidbody rb;
    private SpawnInfo spawnInfo;
    [SerializeField]private ProjectileData projectileData;
    PlayerNetwork playerNetwork;
    PlayerState playerState;
    Vector3 spawnPoint;

   



    public void Initialize(SpawnInfo spawnInfo,Vector3 spawnPoint, Collider shooterCollider,PlayerNetwork playerNetwork,PlayerState playerState)
    {
        this.spawnInfo = spawnInfo;
        this.spawnPoint = spawnPoint;
        this.projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        this.playerNetwork = playerNetwork;
        this.playerState = playerState;

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
        rb.velocity = (spawnInfo.direction.normalized * projectileData.speed) + playerNetwork.playerRb.velocity-playerState.externalForce;
    }

    private void Update()
    {
        if (Vector3.Distance(spawnPoint, transform.position) >= projectileData.range)
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
