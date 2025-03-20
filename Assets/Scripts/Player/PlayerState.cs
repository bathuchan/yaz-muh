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

    public Vector2 oldInput = new Vector2();
    public Vector2 curInput = new Vector2();

    public float movementSpeed = 5.0f;

    public Vector3 oldPosition = new Vector3();

    public uint TickPeriod = 20;

    public PlayerNetwork playerNetwork;

    public Stopwatch tickTimer;

    public NetworkVariable<byte> permissions = new NetworkVariable<byte>(255, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    /* Permissions are sent via bytes, every bit field represents the permission for an action
    *  0 means not allowed, 1 is allowed
    *  --Fields--
    *  0: X input (0x01)
    *  1: Y input (0x02)
    *  2: Z input (0x04)
    */

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
        if(IsServer){
            Vector3 tempForce = new Vector3();
            tempForce.x = curInput.x * movementSpeed;
            tempForce.z = curInput.y * movementSpeed;

            playerNetwork.playerRb.velocity = tempForce;
        }

    }

    void Update()
    {
        if(tickTimer.ElapsedMilliseconds > TickPeriod){
            tickTimer.Restart();
            if(oldPosition != playerNetwork.playerRb.position){
                oldPosition = playerNetwork.playerRb.position;
                UpdatePositionClientRpc(new Vector2(playerNetwork.playerRb.position.x, playerNetwork.playerRb.position.z));
                
            }
        }
    }

    [ClientRpc] 
    void UpdatePositionClientRpc(Vector2 newPosition)
    {
            transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.y);
    }


}
