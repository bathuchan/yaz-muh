using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    
    private PlayerControls playerControls; // Instance of the generated class
    private float moveSpeed = 3f;
    private Vector3 moveDir;

    private void Awake()
    {
        // Create a new instance of PlayerControls for this client
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (IsOwner)
        {
            playerControls.Enable(); // Enable the input actions for the owner
        }
    }

    private void OnDisable()
    {
        if (IsOwner)
        {
            playerControls.Disable(); // Disable the input actions for the owner
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // Read input using the generated class
        moveDir = new Vector3(playerControls.Player.Move.ReadValue<Vector2>().x, 0, playerControls.Player.Move.ReadValue<Vector2>().y);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // Move the player
        Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        transform.position = newPosition;

        // Optionally, you can call a method to update the position on the server
        // UpdatePositionServerRpc(newPosition);
    }

    // Uncomment this if you want to synchronize position with the server
    /*
    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    */
}