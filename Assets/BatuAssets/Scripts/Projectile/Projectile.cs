using Unity.Netcode;
using UnityEngine;


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
    private bool enableVfx = true;
    private PlayerData casterData;

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



    public void Initialize(bool isServerProjectile, SpawnInfo spawnInfo, Collider shooterCollider, PlayerNetwork playerNetwork, PlayerState playerState, /*ulong networkId, */bool visualizeMesh, bool enableVfx)
    {
        this.isServerProjectile = isServerProjectile;
        this.spawnInfo = spawnInfo;
        PlayerDataManager.Instance.TryGetPlayerData(spawnInfo.ownerNetID, out this.casterData);
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

                }
                else if (vfxHandler.despawnVfxs.Length != 0)
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

    NetworkObject otherNetObj;
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
        else if (isServerProjectile&& NetworkManager.Singleton.IsServer)
        {
            if (other.CompareTag("Player") && other.transform.parent.TryGetComponent<NetworkObject>(out otherNetObj))
            {

                if (PlayerDataManager.Instance.TryGetPlayerData(otherNetObj.OwnerClientId, out PlayerData hitPlayer))
                {
                    float updatedHealth = HealthAfterDamage(casterData, hitPlayer);

                    // Server sets the new health value in the synced data
                    PlayerDataManager.Instance.SetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CurrentHealth, updatedHealth);

                    PlayerDataManager.Instance.TryGetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CurrentShield, out float updatedShield);

                    Debug.Log($"[SERVER] {other.name} hit! New health: {updatedHealth}, New shield:{updatedShield}");
                }


                else
                {
                    Debug.Log($"[SERVER] couldn't find {other.name} playerdata!!!");

                }


            }

            Destroy(gameObject);
            return;

        }
    }

    private float DamageAmount(PlayerData casterPlayerData, PlayerData hitPlayerData)
    {
        float baseDamage = projectileData.baseDamage;
        float baseCritChance = projectileData.baseCriticalChange;


        PlayerDataManager.Instance.TryGetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.DamageMultiplier, out float damageMultiplier);
        PlayerDataManager.Instance.TryGetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CriticalChance, out float critChance);
        PlayerDataManager.Instance.TryGetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CriticalMultiplier, out float critMultiplier);
        
        //float damageMultiplier = casterPlayerData.GetStat(PlayerData.PlayerStatType.DamageMultiplier);
        //float critChance = casterPlayerData.GetStat(PlayerData.PlayerStatType.CriticalChance);
        //float critMultiplier = casterPlayerData.GetStat(PlayerData.PlayerStatType.CriticalMultiplier);

        // Calculate if this hit is critical
        bool isCritical = Random.value < (baseCritChance + critChance);

        float totalDamage = baseDamage * (damageMultiplier);
        if (isCritical)
        {
            totalDamage *= (critMultiplier);
            Debug.Log("[DAMAGE] Critical Hit!");
        }

        return totalDamage;
    }

    private float HealthAfterDamage(PlayerData casterPlayerData, PlayerData hitPlayerData)
    {
        PlayerDataManager.Instance.TryGetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CurrentHealth, out float currentHealth);
        PlayerDataManager.Instance.TryGetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CurrentShield, out float shield);
        PlayerDataManager.Instance.TryGetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.ShieldMultiplier, out float shieldMultiplier);

        //float currentHealth = hitPlayerData.GetStat(PlayerData.PlayerStatType.CurrentHealth);
        //float shield = hitPlayerData.GetStat(PlayerData.PlayerStatType.Shield);
        //float shieldMultiplier = hitPlayerData.GetStat(PlayerData.PlayerStatType.ShieldMultiplier);


        float damage = DamageAmount(casterPlayerData, hitPlayerData);

        // Apply shield first
        if (shield > 0)
        {
            float effectiveShield = shield * (1f/* + shieldMultiplier*/);
            if (damage <= effectiveShield)
            {
                float newShield = effectiveShield - damage;

                PlayerDataManager.Instance.SetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CurrentShield, newShield);

                //hitPlayerData.SetStat(PlayerData.PlayerStatType.Shield, newShield);
                return currentHealth; // Health remains unchanged
            }
            else
            {
                damage -= effectiveShield;
                PlayerDataManager.Instance.SetStatValue(otherNetObj.OwnerClientId, PlayerData.PlayerStatType.CurrentShield, 0f);

                //hitPlayerData.SetStat(PlayerData.PlayerStatType.Shield, 0f);
            }
        }

        float newHealth = Mathf.Max(0f, currentHealth - damage);
        return newHealth;
    }


}
