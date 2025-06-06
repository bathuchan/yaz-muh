using System.Collections;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PlayerNetwork : NetworkBehaviour
{
    public PlayerControls playerControls;

    public PlayerState playerState;

    public PlayerAnimationsController playerAnimationsController;

    public Rigidbody playerRb;

    private PlayerData playerData;

    private NetworkObject netObj;

    private WorldUIManager ui;

    [HideInInspector] public Stopwatch inputDeltaTime { get; private set; }
    [HideInInspector] public Stopwatch tickDeltaTime { get; private set; }

    [SerializeField] private Renderer[] modelRenderer;
    private NetworkVariable<int> materialIndex = new NetworkVariable<int>(-1);


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            int index = PlayerColorManager.Instance.AssignUniqueMaterialIndex();
            materialIndex.Value = index;
        }

        ApplyMaterial(materialIndex.Value);

        materialIndex.OnValueChanged += (oldValue, newValue) =>
        {
            ApplyMaterial(newValue);
        };

        ui = GetComponentInChildren<WorldUIManager>();
        if (ui != null)
        {
            ui.OwnerNetId = OwnerClientId;
        }

        if (!IsOwner) return;

        gameObject.name += " (Owner)";
        playerState = GetComponent<PlayerState>();
        playerControls.Enable();

        // Sync all player health/shield info when a new client joins
        if (IsClient||IsHost)
        {
            PlayerDataManager.Instance?.RequestFullPlayerDataSyncServerRpc();
        }
    }


    private void Awake()
    {

        playerControls = new PlayerControls();
        netObj = GetComponent<NetworkObject>();


        playerRb = GetComponent<Rigidbody>();
        playerRb.isKinematic = false;

        playerState = GetComponent<PlayerState>();

        playerAnimationsController = GetComponentInChildren<PlayerAnimationsController>();


        if (inputDeltaTime == null)
        {
            inputDeltaTime = new Stopwatch();
            inputDeltaTime.Start();
        }

        if (tickDeltaTime == null)
        {
            tickDeltaTime = new Stopwatch();
            tickDeltaTime.Start();
        }

    }


    public override void OnNetworkDespawn()
    {
        if (IsServer && materialIndex.Value >= 0)
        {
            PlayerColorManager.Instance.ReleaseMaterialIndex(materialIndex.Value);
        }
    }


    private void Update()
    {
        if (!IsOwner) return;
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    //playerRb.AddForce(transform.forward*100f, ForceMode.Force);
        //}
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    //TakeDamageServerRpc(5);
        //}


    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void ApplyMaterial(int index)
    {
        if (index >= 0 && index < PlayerColorManager.Instance.availableColors.Length)
        {
            Material mat = PlayerColorManager.Instance.GetMaterial(index);
            Material[] currentMaterials = modelRenderer[0].materials;
            currentMaterials[1] = mat;
            modelRenderer[0].materials=currentMaterials;
            currentMaterials = modelRenderer[1].materials;
            currentMaterials[0] = mat;
            modelRenderer[1].materials=currentMaterials;

            gameObject.name = PlayerColorManager.Instance.GetName(index)+" Wizard";
        }
    }

    [ServerRpc]
    public void SendMoveInputToServerRpc(byte moveX, byte moveZ)
    {
        // Apply movement on the server using the stored Rigidbody reference

        playerState.curMoveInput = new Vector2(ConvertByteSignedToFloat(moveX), ConvertByteSignedToFloat(moveZ));


    }

    [ServerRpc]
    public void SendLookInputToServerRpc(byte lookX, byte lookY)
    {
        playerState.curLookInput = new Vector2(ConvertByteSignedToFloat(lookX), ConvertByteSignedToFloat(lookY));
    }



    /*
    Explanation: Converts a signed float value to signed byte, in range (-127,127).
    It will use MSB as sign bit. If MSB is 0, then value is positive, otherwise negative.
    Parameters: Value to be converted as float
    Return: Clamped value as byte in range (-127,127)
    */
    public static byte ConvertFloatToByteSigned(float value)
    {
        // Clamp value to -1 to 1
        value = Mathf.Clamp(value, -1f, 1f);

        // Convert to 7-bit magnitude (0 to 127)
        byte magnitude = (byte)(Mathf.Abs(value) * 127);

        // If negative, set the MSB (Most Significant Bit)
        if (value < 0)
        {
            return (byte)(magnitude | 0x80);
        }

        return magnitude; // Otherwise, return positive magnitude
    }



    public static float ConvertByteSignedToFloat(byte value)
    {
        // Extract sign bit
        bool isNegative = (value & 0x80) != 0;

        // Extract magnitude (clear MSB)
        byte magnitude = (byte)(value & 0x7F);

        // Convert back to float in range (-1 to 1)
        float floatValue = magnitude / 127f;

        return isNegative ? -floatValue : floatValue;
    }



}
