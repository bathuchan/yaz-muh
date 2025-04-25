using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;

public class Projectile : NetworkBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    private SpawnInfo spawnInfo;
    [SerializeField] private ProjectileData projectileData;
    PlayerNetwork playerNetwork;
    PlayerState playerState;
    public NetworkObject networkObject;
    Vector3 spawnPoint;
    ulong networkId;
    [HideInInspector] public MeshRenderer meshRenderer;
    public bool isVisualOnly = false; // Set to true for client-side visuals


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
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("PROJECTILE SPAWNDEN ON SERVER");
        //if (projectileData.trajectoryStyle is CircularTrajectory)
        //{
        //    rb.isKinematic = true; // Don't move
        //}
        //else
        //{
        //    rb.isKinematic = false;
        //}

    }

    public void Initialize(SpawnInfo spawnInfo, Collider shooterCollider, PlayerNetwork playerNetwork, PlayerState playerState, ulong networkId)
    {
        this.spawnInfo = spawnInfo;
        this.spawnPoint = spawnInfo.spawnPoint;
        this.projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        this.playerNetwork = playerNetwork;
        this.playerState = playerState;
        this.networkId = networkId;

        //velocity = spawnInfo.direction.normalized * (projectileData.speed != 0 ? projectileData.speed : moveSpeed);
        //rb.velocity = velocity;


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

        if (isVisualOnly)
        { 
            meshRenderer.enabled = true;
        }
        else 
        {
            meshRenderer.enabled = false;
        }

        if (projectileData.trajectoryStyle is CircularTrajectory)
        {
            rb.isKinematic = true; // Don't move
        }
        else
        {
            rb.isKinematic = false;
        }


        gameObject.SetActive(true);
    }

    private void Start()
    {
        
        //if (IsSpawned) return;

        //meshRenderer.enabled = true;
        //rb.isKinematic = false;

        // Assign velocity for visual-only
        if (isVisualOnly && projectileData.trajectoryStyle != null && !(projectileData.trajectoryStyle is CircularTrajectory))
        {
            //velocity = spawnInfo.direction.normalized * (projectileData.speed != 0 ? projectileData.speed : moveSpeed);
            //rb.velocity = velocity;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null || pathPoints == null || pathPoints.Length == 0) return;
        if (currentPathIndex >= pathPoints.Length)
        {
            if (isVisualOnly)
            {
                Destroy(gameObject);
            }
            else if (IsServer && networkObject.IsSpawned)
            {
                networkObject.Despawn();
            }
            return;
        }

        Vector3 currentPos = rb.position;
        Vector3 target = pathPoints[currentPathIndex];
        float speed = (projectileData.speed != 0 ? projectileData.speed : moveSpeed);
        Vector3 direction = (target - currentPos).normalized;

        // Apply velocity
        rb.velocity = direction * speed;

        // Smooth rotation toward movement direction
        if (direction != Vector3.zero)
        {
            //Quaternion targetRotation = Quaternion.LookRotation(direction);
            //rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.15f));
        }
    }


    private void Update()
    {
        if (rb == null || pathPoints == null || pathPoints.Length == 0) return;
        if (currentPathIndex >= pathPoints.Length) return;

        Vector3 currentPos = rb.position;
        Vector3 target = pathPoints[currentPathIndex];

        // Check distance to advance to next point
        if (Vector3.Distance(currentPos, target) < 1f) 
        {
            currentPathIndex++;
        }
    }






    //private void Update()
    //{
    //    if (pathPoints == null || pathPoints.Length == 0) return;

    //    if (currentPathIndex >= pathPoints.Length)
    //    {
    //        if (isVisualOnly)
    //        {
    //            Destroy(gameObject); // Visual projectiles are local
    //        }
    //        else if (IsServer && networkObject.IsSpawned)
    //        {
    //            networkObject.Despawn(); // Authoritative cleanup
    //        }

    //        return;
    //    }

    //    Vector3 target = pathPoints[currentPathIndex];
    //    float step = (projectileData.speed != 0 ? projectileData.speed : moveSpeed) * Time.deltaTime;

    //    transform.position = Vector3.MoveTowards(transform.position, target, step);

    //    if (Vector3.Distance(transform.position, target) < 0.05f)
    //    {
    //        currentPathIndex++;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (isVisualOnly)
        {
            Destroy(gameObject); // Destroy locally for visual projectiles
            return;
        }

        if (IsServer)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"[SERVER] Projectile hit {other.name}");
                // Apply damage/effects here if needed
            }

            if (networkObject.IsSpawned)
            {
                networkObject.Despawn();
            }
        }
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
