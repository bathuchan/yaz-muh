
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : NetworkBehaviour
{
    PlayerNetwork playerNetwork;
    public Vector2 speed;

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

        speed = context.ReadValue<Vector2>();



    }

    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        speed = Vector2 .zero;

    }

    private void OnDisable()//Remove listeners
    {
        playerNetwork.playerControls.Player.Move.performed -= OnMove;
        playerNetwork.playerControls.Player.Move.canceled -= OnMoveCancel;

    }
}
