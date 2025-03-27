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
    public Vector2 oldLookInput = new Vector2();

    public Vector2 curMoveInput = new Vector2();

    public float movementSpeed = 5.0f;

    public Vector3 oldPositionServer = new Vector3();

    public NetworkVariable<float> NetworkYRotation = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );


    public Vector2 curLookInput = Vector2.zero;
    public float rotationSpeed = 10f;

    private PlayerLook playerLook;
    private PlayerMovement playerMove;

    public uint TickPeriod = 20;

    public PlayerNetwork playerNetwork;
    public Stopwatch tickTimer;

    public NetworkVariable<byte> permissions = new NetworkVariable<byte>(255, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        playerLook = GetComponentInChildren<PlayerLook>();
        playerMove = GetComponentInChildren<PlayerMovement>();
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
            // Smoothly apply received Y rotation
            Quaternion targetRotation = Quaternion.Euler(0, NetworkYRotation.Value, 0);
            playerLook.playerModel.rotation = Quaternion.Slerp(
                playerLook.playerModel.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed
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
                // Check for position updates
                if (oldPositionServer != playerNetwork.playerRb.position)
                {
                    oldPositionServer = playerNetwork.playerRb.position;
                    UpdatePositionClientRpc(new Vector2(playerNetwork.playerRb.position.x, playerNetwork.playerRb.position.z));
                }

                // Determine target rotation
                Quaternion newRotation = playerLook.playerModel.rotation;

                // Calculate Y rotation based on movement or look input
                if (curLookInput != Vector2.zero)
                {
                    float targetAngle = Mathf.Atan2(curLookInput.x, curLookInput.y) * Mathf.Rad2Deg;
                    NetworkYRotation.Value = targetAngle;
                }
                else if (curMoveInput != Vector2.zero)
                {
                    float targetAngle = Mathf.Atan2(curMoveInput.x, curMoveInput.y) * Mathf.Rad2Deg;
                    NetworkYRotation.Value = targetAngle;
                }


                // Only send an update if rotation has changed
                //if (oldRotation != newRotation)
                //{
                //    oldRotation = newRotation;
                //    NetworkRotation.Value = newRotation;  // Set the NetworkVariable
                //}

            }
        }


    }


    [ClientRpc]
    void UpdatePositionClientRpc(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.y);
    }

    //[ClientRpc]
    //void UpdateRotationClientRpc(float newRotationY)
    //{
    //    targetRotation = newRotation; 
    //}


}
