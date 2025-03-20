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
    
    public Vector3 moveDir { get; private set; }
    public Rigidbody playerRb;

    private Stopwatch inputDeltaTime;
    private Stopwatch tickDeltaTime;



    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>(
       new PlayerData(100, 50),
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server
   );

    public override void OnNetworkSpawn()
    {

        randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log("Player id:" + OwnerClientId + " - randomNumber: " + randomNumber.Value);
        };
        playerData.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"Player id:{OwnerClientId} Health: {newValue.currentHealth}, Shield: {newValue.shield}");
        };

        if (!IsOwner) return;
        playerState = GetComponent<PlayerState>();

       


        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMoveCancel;
        playerControls.Enable();


    }


    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.isKinematic = false;
        playerState = GetComponent<PlayerState>();

        playerControls = new PlayerControls();
        if(inputDeltaTime == null){
            inputDeltaTime = new Stopwatch();
            inputDeltaTime.Start();
        }

        if(tickDeltaTime == null){
            tickDeltaTime = new Stopwatch();
            tickDeltaTime.Start();
        }
         
    }
    

    private void OnMove(InputAction.CallbackContext context)
    {
        if(inputDeltaTime.ElapsedMilliseconds > playerState.TickPeriod){
            Vector2 speed = context.ReadValue<Vector2>();
            SendInputToServerRpc(ConvertFloatToByteSigned(speed.x), ConvertFloatToByteSigned(speed.y));
            inputDeltaTime.Restart();
        }
        

    }

    private void OnMoveCancel(InputAction.CallbackContext context)
    {
            SendInputToServerRpc(0, 0); // TODO: We don't need parameters. Create a new RPC that takes no parameter and does same thing.
    }

    private void OnDisable()//remove listeners
    {
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMoveCancel;
        playerControls.Disable();
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = Random.Range(0, 100);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            //TakeDamageServerRpc(5);
        }


    }

    private void FixedUpdate()
    {

    }

    [ServerRpc]
    private void SendInputToServerRpc(byte x, byte z)
    {
        // Apply movement on the server using the stored Rigidbody reference

        playerState.curInput = new Vector2(ConvertByteSignedToFloat(x), ConvertByteSignedToFloat(z));

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
