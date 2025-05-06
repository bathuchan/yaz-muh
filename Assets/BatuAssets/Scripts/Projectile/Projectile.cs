using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    private SpawnInfo spawnInfo;
    [SerializeField] private ProjectileData projectileData;
    PlayerNetwork playerNetwork;
    PlayerState playerState;
    //public NetworkObject networkObject;
    Vector3 spawnPoint;
    //ulong networkId;
    [HideInInspector] public MeshRenderer meshRenderer;
    [HideInInspector] public bool visualizeMesh = false; // Set to true for client-side visuals
    private bool isServerProjectile = false;
    private ProjectileVFXHandler vfxHandler;
    private bool enableVfx=true;
    

    private Vector3[] pathPoints;
    private int currentPathIndex = 0;
    private float moveSpeed = 10f; // fallback if projectileData.speed isn't used

    //public NetworkVariable<ulong> networkId = new NetworkVariable<ulong>(
    //    0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    //);


    private void Awake()
    {
       // networkObject = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        vfxHandler = GetComponentInChildren<ProjectileVFXHandler>();
    }



    public void Initialize(bool isServerProjectile,SpawnInfo spawnInfo, Collider shooterCollider, PlayerNetwork playerNetwork, PlayerState playerState, /*ulong networkId, */bool visualizeMesh,bool enableVfx)
    {
        this.isServerProjectile = isServerProjectile;
        this.spawnInfo = spawnInfo;
        this.spawnPoint = spawnInfo.spawnPoint;
        this.projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        this.playerNetwork = playerNetwork;
        this.playerState = playerState;
        //this.networkId = networkId;
        this.visualizeMesh = visualizeMesh;
        this.enableVfx = enableVfx;

        //velocity = spawnInfo.direction.normalized * (projectileData.speed != 0 ? projectileData.speed : moveSpeed);
        //rb.velocity = velocity;

        SetUpProjectile();

        

        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null && shooterCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider);
        }

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

    }


    private void FixedUpdate()
    {
        if (rb == null || pathPoints == null || pathPoints.Length == 0) return;
        if (currentPathIndex >= pathPoints.Length)
        {
            if (vfxHandler != null)
            {
                if (vfxHandler.useImpactVfxOnDespawn)
                {
                    vfxHandler.ImpactVFX();

                }else if (vfxHandler.despawnVfxs.Length != 0) 
                {
                    vfxHandler.DespawnVFX();
                }

                vfxHandler.DetachVFX();
            }

            Destroy(gameObject);
            return;
        }

        Vector3 currentPos = rb.position;
        Vector3 target = pathPoints[currentPathIndex];
        float step = (projectileData.speed != 0 ? projectileData.speed : moveSpeed) * Time.fixedDeltaTime;

        Vector3 newPos = Vector3.MoveTowards(currentPos, target, step);
        rb.MovePosition(newPos); // Smoothed + physics aware

        if (Vector3.Distance(newPos, target) < 0.05f)
        {
            currentPathIndex++;
        }

        // Optional: Rotate toward direction
        Vector3 direction = (target - currentPos).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.2f));
        }
    }

    private void SetUpProjectile() 
    {
        transform.SetParent(ContainerController.Instance.GetContainer("Projectile"));

        if (visualizeMesh)
        {

            meshRenderer.enabled = true;
            if (isServerProjectile)
            {

                meshRenderer.material.color = Color.white;
                meshRenderer.material.DisableKeyword("_EMISSION");
               

            }
        }
        else
        {
            meshRenderer.enabled = false;
            
        }

        

        if (projectileData.trajectoryStyle != null && projectileData.trajectoryStyle is CircularTrajectory ? false : true)
        {
            pathPoints = projectileData.trajectoryStyle.CalculateTrajectory(spawnPoint, spawnInfo.direction, projectileData.speed, projectileData.range);
            currentPathIndex = 0;
        }



        if (projectileData.trajectoryStyle is CircularTrajectory)
        {
            rb.isKinematic = true; // Don't move
        }
        else
        {
            rb.isKinematic = false;
        }



        if (enableVfx && vfxHandler != null)
        {
            vfxHandler.InitializeVFX();
        }
        else 
        {
            vfxHandler = null;
        }
        

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isServerProjectile)
        {
            if (vfxHandler != null)
            {
                vfxHandler.ImpactVFX();
                vfxHandler.DetachVFX();
            }

            Destroy(gameObject); // Destroy locally for visual projectiles
            return;
        }
        else if (isServerProjectile)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"[SERVER] Projectile hit {other.name}");
                // Apply damage/effects here if needed
            }
          
            Destroy(gameObject);
            return;
          
        }
    }



}
