
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : NetworkBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;

    private AudioListener audioListener;

    PlayerNetwork playerNetwork;

    public Vector2 lookInput;

    public float rotationSpeed = 5f;

    public Transform playerModel; // The model that should rotate
    private void Awake()
    {
        cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();
        audioListener = GetComponent<AudioListener>();
        playerNetwork = GetComponentInParent<PlayerNetwork>();
    }



    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        FindAndAssignCamera();

        audioListener.enabled = true;

        if (playerNetwork == null || playerNetwork.playerControls == null) return;//rotation listeners set on joining server

        playerNetwork.playerControls.Player.Look.performed += OnLook;
        playerNetwork.playerControls.Player.Look.canceled += OnLookCancel;

    }

    private void FindAndAssignCamera()
    {
        if (cinemachineCamera == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found in the scene!");
            return;
        }

        // Set the virtual camera to follow and look at the player
        cinemachineCamera.Follow = transform;
        cinemachineCamera.LookAt = transform;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        
            lookInput = context.ReadValue<Vector2>();

        
    }
    private void OnLookCancel(InputAction.CallbackContext context)
    {

        lookInput = Vector2.zero;


    }



    private void OnDisable()
    {
        if (playerNetwork == null || playerNetwork.playerControls == null) return;

        playerNetwork.playerControls.Player.Look.performed -= OnLook;
        playerNetwork.playerControls.Player.Look.canceled -= OnLookCancel;

    }
}
