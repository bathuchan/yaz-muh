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
        abilityJoysticks=GameObject.FindObjectsOfType<AbilityJoystick>();
        playerState = GetComponent<PlayerState>();
        playerLook= GetComponent<PlayerLook>();
        //trajectoryLine= GetComponent<LineRenderer>();
    }
    private void Start()
    {
        playerNetwork = GetComponent<PlayerNetwork>();
        shooterCollider = GetComponentInChildren<Collider>();
        currentProjectileData= ProjectileDatabase.Instance.GetProjectileData(projectileId);
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            projectileId = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            projectileId = 2;
        }

        // Cast ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CastAbility();
        }

    }

    public void CastAbility() 
    {
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned!");
            return;
        }
    

        SpawnInfo spawnInfo = new SpawnInfo
        {
            projectileId = this.projectileId,

            direction = playerLook.playerModel.transform.forward

        };


        Debug.Log("[CLIENT] Requesting Server to spawn projectile on all clients...");
        RequestProjectileSpawnServerRpc(spawnInfo);

        playerNetwork.playerAnimationsController.TriggerAttackAnimation();
    }

    [ServerRpc]
    private void RequestProjectileSpawnServerRpc(SpawnInfo spawnInfo)
    {
        
        Debug.Log($"[SERVER] Received request to spawn projectile {spawnInfo.projectileId}");

        //TODO:somehow need to create a projectile instance on serverside as well and check if client side projectile and this serverside projectile matchs.
        //Or only trust the serverside projectile to register collisions

        ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        GameObject projectileInstance = Instantiate(
            projectileData.prefab,
            firePoint.position,
            Quaternion.identity
        );
        projectileInstance.name = "ServerProj";
        int networkId = projectileInstance.gameObject.GetInstanceID();
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {

            projectile.Initialize(spawnInfo, firePoint.position, shooterCollider, playerNetwork, playerState, networkId);
            projectile.networkObject.Spawn();

        }
        else
        {
            Debug.LogError("[SERVER] Spawned projectile is missing the Projectile component!");
        }

        SpawnProjectileClientRpc(spawnInfo, networkId);
    }


    [ClientRpc]
    private void SpawnProjectileClientRpc(SpawnInfo spawnInfo,int networkId)
    {
        Debug.Log($"[CLIENT] Received SpawnProjectileClientRpc! Projectile network ID: {networkId}");

        ProjectileData projectileData = ProjectileDatabase.Instance.GetProjectileData(spawnInfo.projectileId);
        if (projectileData == null)
        {
            Debug.LogError("[SERVER] Invalid projectile ID!");
            return;
        }

        GameObject projectileInstance = Instantiate(
            projectileData.prefab,
            firePoint.position,
            Quaternion.identity
        );
        projectileInstance.name = "CLientProj";

        // Initialize projectile with shooter collider

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {
            
            projectile.Initialize(spawnInfo, firePoint.position, shooterCollider,playerNetwork,playerState,networkId);

        }
        else
        {
            Debug.LogError("[SERVER] Spawned projectile is missing the Projectile component!");
        }
    }



}
