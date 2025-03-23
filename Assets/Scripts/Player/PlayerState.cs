using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    private const byte INPUT_X_PERM = 0x01;
    private const byte INPUT_Y_PERM = 0x02;
    private const byte INPUT_Z_PERM = 0x04;

    public Vector2 oldMoveInput = new Vector2();
    public Vector2 curMoveInput = new Vector2();

    public float movementSpeed = 5.0f;

    public Vector3 oldPosition = new Vector3();
    public Quaternion oldRotation = Quaternion.identity;
    private Quaternion targetRotation = Quaternion.identity; // Stores the latest received rotation target

    public Vector2 curLookInput = Vector2.zero;
    public float rotationSpeed = 10f;

    private PlayerLook playerLook;

    public uint TickPeriod = 20;

    public PlayerNetwork playerNetwork;
    public Stopwatch tickTimer;

    public NetworkVariable<byte> permissions = new NetworkVariable<byte>(255, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        playerLook = GetComponentInChildren<PlayerLook>();
    }

    public void Start()
    {
        tickTimer = new Stopwatch();
        tickTimer.Start();
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
            // Apply movement
            Vector3 tempForce = new Vector3();
            tempForce.x = curMoveInput.x * movementSpeed;
            tempForce.z = curMoveInput.y * movementSpeed;
            playerNetwork.playerRb.velocity = tempForce;
        }

        if (IsClient && playerLook != null && playerLook.playerModel != null)
        {
            // Smoothly rotate the player model to the latest target rotation
            playerLook.playerModel.rotation = Quaternion.Slerp(playerLook.playerModel.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    void Update()
    {
        if (tickTimer.ElapsedMilliseconds > TickPeriod)
        {
            tickTimer.Restart();

            if (IsServer)
            {
                // Check for position updates
                if (oldPosition != playerNetwork.playerRb.position)
                {
                    oldPosition = playerNetwork.playerRb.position;
                    UpdatePositionClientRpc(new Vector2(playerNetwork.playerRb.position.x, playerNetwork.playerRb.position.z));
                }

                // Determine target rotation
                Quaternion newRotation = playerLook.playerModel.rotation;

                if (curLookInput != Vector2.zero)
                {
                    // Rotate based on look input
                    float targetAngle = Mathf.Atan2(curLookInput.x, curLookInput.y) * Mathf.Rad2Deg;
                    newRotation = Quaternion.Euler(0, targetAngle, 0);
                }
                else if (curMoveInput != Vector2.zero)
                {
                    // Rotate based on movement direction if no look input
                    float targetAngle = Mathf.Atan2(curMoveInput.x, curMoveInput.y) * Mathf.Rad2Deg;
                    newRotation = Quaternion.Euler(0, targetAngle, 0);
                }

                // Only send an update if rotation has changed
                if (oldRotation != newRotation)
                {
                    oldRotation = newRotation;
                    UpdateRotationClientRpc(newRotation);
                }
            }
        }
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.y);
    }

    [ClientRpc]
    void UpdateRotationClientRpc(Quaternion newRotation)
    {
        targetRotation = newRotation; 
    }

    
}
