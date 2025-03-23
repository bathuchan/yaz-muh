
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : NetworkBehaviour
{
    PlayerNetwork playerNetwork;

    private void Awake()
    {
        playerNetwork = GetComponent<PlayerNetwork>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (playerNetwork != null && playerNetwork.playerControls != null) 
        {
            playerNetwork.playerControls.Player.Move.performed += OnMove;
            playerNetwork.playerControls.Player.Move.canceled += OnMoveCancel;
            
        }
           
    }


    private void OnMove(InputAction.CallbackContext context)
    {
        if (playerNetwork.inputDeltaTime.ElapsedMilliseconds > playerNetwork.playerState.TickPeriod)
        {
            Vector2 speed = context.ReadValue<Vector2>();
            playerNetwork.SendMoveInputToServerRpc(PlayerNetwork.ConvertFloatToByteSigned(speed.x), PlayerNetwork.ConvertFloatToByteSigned(speed.y));
            playerNetwork.inputDeltaTime.Restart();
        }


    }

    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        playerNetwork.SendMoveInputToServerRpc(0, 0); // TODO: We don't need parameters. Create a new RPC that takes no parameter and does same thing.
    }

    private void OnDisable()//Remove listeners
    {
        playerNetwork.playerControls.Player.Move.performed -= OnMove;
        playerNetwork.playerControls.Player.Move.canceled -= OnMoveCancel;
        
    }
}
