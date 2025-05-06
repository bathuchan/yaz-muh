using Unity.Netcode;
using UnityEngine;


public class PlayerAbility : NetworkBehaviour
{
    [SerializeField] public int projectileId; // Set this ID in Inspector or use keys "1-2" to change projectile type
    [SerializeField] public Transform firePoint;

    public PlayerNetwork playerNetwork;
    public PlayerState playerState;
    Collider shooterCollider;
    public PlayerLook playerLook;
    //public LineRenderer trajectoryLine; // LineRenderer for path visualization

    public AbilityJoystick[] abilityJoysticks;

    public ProjectileData currentProjectileData;
    private void Awake()
    {
        abilityJoysticks = GameObject.FindObjectsOfType<AbilityJoystick>();
        playerState = GetComponent<PlayerState>();
        playerLook = GetComponent<PlayerLook>();
        //trajectoryLine= GetComponent<LineRenderer>();
    }
    private void Start()
    {
        playerNetwork = GetComponent<PlayerNetwork>();
        shooterCollider = GetComponentInChildren<Collider>();
        currentProjectileData = ProjectileDatabase.Instance.GetProjectileData(projectileId);
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        for (int i = 0; i < abilityJoysticks.Length; i++)
        {
            abilityJoysticks[i].playerAbility = this;

        }


    }


    private void Update()
    {
        if (!IsOwner) return;

        // Change casted ability
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    projectileId = 1;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    projectileId = 2;
        //}

        //// Cast ability
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    CastAbility();
        //}

    }

    public void CastAbility()
    {
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned!");
            return;
        }


        Vector3 spawnPosition = currentProjectileData.trajectoryStyle is CircularTrajectory
           ? playerLook.trajectoryManager.currentAimPosition.position + Vector3.up * 0.95f
           : firePoint.position;

        SpawnInfo spawnInfo = new SpawnInfo
        {
            projectileId = this.projectileId,

            direction = playerLook.playerModel.transform.forward,

            spawnPoint = spawnPosition

        };


        Debug.Log("[CLIENT] Requesting Server to spawn projectile on all clients...");
        RequestProjectileSpawnServerRpc(spawnInfo);

        playerNetwork.playerAnimationsController.TriggerAttackAnimation();
    }

    [ServerRpc]
    private void RequestProjectileSpawnServerRpc(SpawnInfo spawnInfo)
    {
        Debug.Log($"[SERVER] Received request to spawn projectile {spawnInfo.projectileId}");

        ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
       

        GameObject projectileInstance = Instantiate(
            projectileData.prefab,
            spawnInfo.spawnPoint,
            Quaternion.identity
        );
        projectileInstance.name = "ServerProj";

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {
            //ulong networkId = projectile.networkObject.NetworkObjectId;

            //change to each bool value for testing purposes for visual testings
          
            projectile.Initialize( false ,spawnInfo, shooterCollider, playerNetwork, playerState,/* networkId,*/ true,true);

            // Tell clients to spawn visual projectile
            SpawnVisualProjectileClientRpc(spawnInfo);
        }
    }

    //[ClientRpc]
    //private void InitializeProjectileClientRpc(SpawnInfo spawnInfo, Vector3 spawnPoint, ulong networkId, ulong netObjId)
    //{
    //    // This runs on each client after the projectile is spawned
    //    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(netObjId, out NetworkObject projectileNetObj))
    //    {
    //        Projectile projectile = projectileNetObj.GetComponent<Projectile>();
    //        if (projectile != null)
    //        {
    //            Collider shooterCollider = GetComponentInChildren<Collider>(); // Or find another way to pass this
    //            projectile.Initialize(spawnInfo, spawnPoint, shooterCollider, playerNetwork, playerState, networkId);
    //            projectile.isVisualOnly = false;
    //        }
    //        else
    //        {
    //            Debug.LogError("[CLIENT] Could not find Projectile script on spawned object.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("[CLIENT] Could not find NetworkObject with ID: " + netObjId);
    //    }
    //}

    [ClientRpc]
    private void SpawnVisualProjectileClientRpc(SpawnInfo spawnInfo)
    {
        if (IsServer) return; //  Server already has its own projectile

        ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        if (projectileData == null)
        {
            Debug.LogError("[CLIENT] Invalid projectile ID for visual spawn!");
            return;
        }
      

        GameObject visualProjectile = Instantiate(
            projectileData.prefab,
            spawnInfo.spawnPoint,
            Quaternion.identity
        );
        visualProjectile.name = "VisualClientProjectile";
        //visualProjectile.transform.SetParent(ContainerController.Instance.GetContainer("Projectile"));

        Projectile projectile = visualProjectile.GetComponent<Projectile>();
        if (projectile != null)
        {
            //ulong networkId = projectile.networkObject.NetworkObjectId;
          
           
            
            projectile.Initialize(false,spawnInfo, shooterCollider, playerNetwork, playerState,/* networkId,*/ true , true);
           
        }

    }



    //[ClientRpc]
    //private void SpawnProjectileClientRpc(SpawnInfo spawnInfo, ulong networkId)
    //{
    //    Debug.Log($"[CLIENT] Received SpawnProjectileClientRpc! Projectile network ID: {networkId}");

    //    ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
    //    if (projectileData == null)
    //    {
    //        Debug.LogError("[SERVER] Invalid projectile ID!");
    //        return;
    //    }

    //    Vector3 spawnPosition = projectileData.trajectoryStyle is CircularTrajectory ?
    // playerLook.trajectoryManager.currentAimPosition.position : firePoint.position;

    //    GameObject projectileInstance = Instantiate(
    //        projectileData.prefab,
    //        spawnPosition,
    //        Quaternion.identity
    //    );
    //    projectileInstance.name = "CLientProj";

    //    // Initialize projectile with shooter collider

    //    Projectile projectile = projectileInstance.GetComponent<Projectile>();
    //    if (projectile != null)
    //    {

    //        projectile.Initialize(spawnInfo, shooterCollider, playerNetwork, playerState, networkId,true);

    //    }
    //    else
    //    {
    //        Debug.LogError("[SERVER] Spawned projectile is missing the Projectile component!");
    //    }
    //}



}
