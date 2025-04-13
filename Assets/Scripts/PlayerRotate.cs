using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotate : NetworkBehaviour
{
    private PlayerNetwork playerNetwork; // Reference to PlayerNetwork
    private Vector2 lookInput;
    
    public float rotationSpeed = 5f;

    [SerializeField] private Transform playerModel; // The model that should rotate (capsule)

    private NetworkVariable<Quaternion> syncedRotation = new NetworkVariable<Quaternion>(
        Quaternion.identity,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );


    private void Awake()
    {
        playerNetwork = GetComponentInParent<PlayerNetwork>();
        playerModel = transform;
    }

    private void OnEnable()
    {
        if (playerNetwork == null || playerNetwork.playerControls == null) return;

        playerNetwork.playerControls.Player.Look.performed += OnLook;
        playerNetwork.playerControls.Player.Look.canceled += OnLookCancel;
        
    }

    private void OnDisable()
    {
        if (playerNetwork == null || playerNetwork.playerControls == null) return;

        playerNetwork.playerControls.Player.Look.performed -= OnLook;
        playerNetwork.playerControls.Player.Look.canceled -= OnLookCancel;
       
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookCancel(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

   

    private void Update()
    {
        if (IsOwner)
        {
            RotatePlayer();
        }
        else
        {
            // Non-owner clients smoothly interpolate towards the synced rotation
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, syncedRotation.Value, Time.deltaTime * rotationSpeed);
        }
    }
    private void RotatePlayer()
    {
        Vector3 targetDirection = Vector3.zero;

        if (lookInput.sqrMagnitude > 0.01f)
        {
            targetDirection = lookInput;
        }
        else if (playerNetwork.moveDir.sqrMagnitude > 0.01f)
        {
            targetDirection = playerNetwork.moveDir;
        }

        if (targetDirection.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Send rotation to the server
            SendRotationToServerRpc(playerModel.rotation);
        }
    }

    [ServerRpc]
    private void SendRotationToServerRpc(Quaternion rotation)
    {
        syncedRotation.Value = rotation;
    }

}
