using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;

public class Projectile : NetworkBehaviour
{
    private Rigidbody rb;
    private SpawnInfo spawnInfo;
    [SerializeField] private ProjectileData projectileData;
    PlayerNetwork playerNetwork;
    PlayerState playerState;
    public NetworkObject networkObject;
    Vector3 spawnPoint;
    ulong networkId;

    private Vector3[] pathPoints;
    private int currentPathIndex = 0;
    private float moveSpeed = 10f; // fallback if projectileData.speed isn't used

    //public NetworkVariable<ulong> networkId = new NetworkVariable<ulong>(
    //    0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    //);


    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (projectileData.trajectoryStyle is CircularTrajectory)
        {
            rb.isKinematic = true; // Don't move
        }
        else
        {
            rb.isKinematic = false;
        }

    }

    public void Initialize(SpawnInfo spawnInfo, Vector3 spawnPoint, Collider shooterCollider, PlayerNetwork playerNetwork, PlayerState playerState, ulong networkId)
    {
        this.spawnInfo = spawnInfo;
        this.spawnPoint = spawnPoint;
        this.projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        this.playerNetwork = playerNetwork;
        this.playerState = playerState;
        this.networkId = networkId;

        // rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // we'll control movement manually

        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null && shooterCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider);
        }

        if (projectileData.trajectoryStyle != null && projectileData.trajectoryStyle is CircularTrajectory ? false : true)
        {
            pathPoints = projectileData.trajectoryStyle.CalculateTrajectory(spawnPoint, spawnInfo.direction, projectileData.speed, projectileData.range);
            currentPathIndex = 0;
        }

        gameObject.SetActive(true);
    }

    private void Start()
    {

    }

    private void Update()
    {
        if ((pathPoints == null || pathPoints.Length == 0) && networkObject.IsSpawned) { return; }
        else if (currentPathIndex == pathPoints.Length)
        {
            if (IsServer) networkObject.Despawn();
            if (IsClient) this.gameObject.SetActive(false);
            return;
        }

        if (currentPathIndex >= pathPoints.Length)
        {


            //Destroy(gameObject);

        }

        // Move toward the next point
        Vector3 target = pathPoints[currentPathIndex];
        float step = (projectileData.speed != 0 ? projectileData.speed : moveSpeed) * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            currentPathIndex++;

        }
        //if (Vector3.Distance(spawnPoint, transform.position) >= projectileData.range)
        //{
        //    Destroy(gameObject);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsClient) 
        {
            this.gameObject.SetActive(false);
        }

        if (IsServer)
        {

            if (other.CompareTag("Player"))
            {
                Debug.Log($"[SERVER] Projectile hit {other.name}");

                // Apply effects or damage to player if needed
                //DestroyProjectileServerRpc();
            }
            else
            {
                networkObject.Despawn();
                //Destroy(gameObject);
            }
        }
        //DestroyProjectileServerRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc()
    {
        // Send the projectileId to clients so they know which one to destroy
        DestroyProjectileClientRpc(networkId);

        // Destroy the projectile on the server
        Destroy(gameObject);
    }


    [ClientRpc]
    private void DestroyProjectileClientRpc(ulong projectileId)
    {
        // Look for the projectile with the given projectileId
        //if (CurrentProjectiles.Instance.activeProjectiles.ContainsKey(projectileId))
        //{
        //    GameObject projectileInstance = CurrentProjectiles.Instance.activeProjectiles[projectileId];
        //    if (projectileInstance != null)
        //    {
        //        // Destroy the projectile locally
        //        Destroy(projectileInstance);
        //        CurrentProjectiles.Instance.activeProjectiles.Remove(projectileId); // Remove it from the dictionary
        //    }
        //}
        //else
        //{
        //    Debug.LogWarning("[CLIENT] Projectile not found with ID: " + projectileId);
        //}
    }
}
