using System;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    private const byte INPUT_X_PERM = 0x01;
    private const byte INPUT_Y_PERM = 0x02;
    private const byte INPUT_Z_PERM = 0x04;

    public Vector2 oldMoveInput = Vector2.zero;
    public Vector2 oldLookInput = Vector2.zero;
    public Vector2 curMoveInput = Vector2.zero;
    public Vector2 curLookInput = Vector2.zero;

    public float movementSpeed = 5.0f;

    private Vector3 targetPosition; // Interpolation Target Position
    private float targetYRotation;  // Interpolation Target Rotation

    public NetworkVariable<float> NetworkYRotation = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public NetworkVariable<byte> permissions = new NetworkVariable<byte>(255, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private PlayerLook playerLook;
    private PlayerMovement playerMove;
    public PlayerNetwork playerNetwork;
    public Stopwatch tickTimer;

    public uint TickPeriod = 20;
    public float interpolationSpeed = 10f; // Adjust for smoothness

    private void Awake()
    {
        playerLook = GetComponentInChildren<PlayerLook>();
        playerMove = GetComponentInChildren<PlayerMovement>();
    }

    private void Start()
    {
        tickTimer = new Stopwatch();
        tickTimer.Start();

        targetPosition = transform.position; // Initialize
        targetYRotation = 0f;

        // Listen for Rotation Updates
        NetworkYRotation.OnValueChanged += (oldValue, newValue) =>
        {
            targetYRotation = newValue; // Update target rotation
        };
    }

    public override void OnNetworkSpawn()
    {
        playerNetwork = GetComponent<PlayerNetwork>();

        if (IsServer)
        {
            permissions.Value = 0xFF; // Example: Full permissions
        }
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            // Apply movement on server
            Vector3 tempForce = new Vector3(curMoveInput.x * movementSpeed, 0, curMoveInput.y * movementSpeed);
            playerNetwork.playerRb.velocity = tempForce;
        }

        if (IsClient && playerLook != null && playerLook.playerModel != null)
        {
            // Interpolate Position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * interpolationSpeed);

            // Rotate Only the Model (Not the Whole Player)
            Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
            playerLook.playerModel.rotation = Quaternion.Slerp(
                playerLook.playerModel.rotation, targetRotation, Time.fixedDeltaTime * interpolationSpeed
            );
        }
    }

    void Update()
    {
        if (tickTimer.ElapsedMilliseconds > TickPeriod)
        {
            tickTimer.Restart();

            if (IsOwner)
            {
                if (oldMoveInput != playerMove.speed)
                {
                    oldMoveInput = playerMove.speed;
                    playerNetwork.SendMoveInputToServerRpc(PlayerNetwork.ConvertFloatToByteSigned(playerMove.speed.x), PlayerNetwork.ConvertFloatToByteSigned(playerMove.speed.y));
                }

                if (oldLookInput != playerLook.lookInput)
                {
                    oldLookInput = playerLook.lookInput;
                    playerNetwork.SendLookInputToServerRpc(PlayerNetwork.ConvertFloatToByteSigned(playerLook.lookInput.x), PlayerNetwork.ConvertFloatToByteSigned(playerLook.lookInput.y));
                }
            }

            if (IsServer)
            {
                // Update Position
                if (targetPosition != playerNetwork.playerRb.position)
                {
                    targetPosition = playerNetwork.playerRb.position;
                    UpdatePositionClientRpc(new Vector2(targetPosition.x, targetPosition.z));
                }

                // 🔹 Update Rotation (Server Sets Rotation)
                if (curLookInput != Vector2.zero)
                {
                    float targetAngle = Mathf.Atan2(curLookInput.x, curLookInput.y) * Mathf.Rad2Deg;
                    NetworkYRotation.Value = targetAngle; // Sync rotation to clients
                }
                else if (curMoveInput != Vector2.zero)
                {
                    float targetAngle = Mathf.Atan2(curMoveInput.x, curMoveInput.y) * Mathf.Rad2Deg;
                    NetworkYRotation.Value = targetAngle;
                }
            }
        }
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector2 newPosition)
    {
        targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.y);
    }
}
