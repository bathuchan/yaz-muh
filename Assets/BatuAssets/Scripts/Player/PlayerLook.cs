
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public class PlayerLook : NetworkBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;

    private AudioListener audioListener;

    PlayerNetwork playerNetwork;

    public Vector2 lookInput;

    public float rotationSpeed = 5f;

    public Transform playerModel; // The model that should rotate

    
    public Transform aimPoint;

    public PlayerAbility playerAbility;

    [HideInInspector]public TrajectoryManager trajectoryManager;


    private void Awake()
    {
        cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();
        audioListener = GetComponent<AudioListener>();
        playerNetwork = GetComponent<PlayerNetwork>();
        playerAbility = GetComponent<PlayerAbility>();
        trajectoryManager = GetComponentInChildren<TrajectoryManager>();
    }




    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        FindAndAssignCamera();

        aimPoint.position = Vector3.zero;

        audioListener.enabled = true;

        if (playerNetwork == null || playerNetwork.playerControls == null) return;//rotation listeners set on joining server

        playerNetwork.playerControls.Player.Look.performed += OnLook;
        playerNetwork.playerControls.Player.Look.canceled += OnLookCancel;

    }

    private Vector3 aimVelocity = Vector3.zero; // Needed for SmoothDamp

    private void Update()
    {
        if (!IsOwner) return;

        

        if (playerModel != null && playerAbility.currentProjectileData != null)
        {
            float range = playerAbility.currentProjectileData.range;

            if (lookInput.magnitude > 0.01f)
            {
                Vector3 aimDirection = playerModel.forward;
                Vector3 targetPosition = playerModel.position + aimDirection * range * lookInput.magnitude + Vector3.down * 0.95f;

                aimPoint.position = Vector3.SmoothDamp(
                    aimPoint.position,
                    targetPosition,
                    ref aimVelocity,
                    0.2f
                );
            }
            else
            {
                // Snap to zero if no input
                aimPoint.position = playerModel.position +Vector3.down*0.95f;
            }

        }
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

        //trajectoryManager.StartTrajectory(); // Start preview
    }

    private void OnLookCancel(InputAction.CallbackContext context)
    {
      
        //trajectoryManager.StopTrajectory();
        lookInput = Vector2.zero;
        //aimPoint.transform.position = Vector3.zero;
        
    }




    private void OnDisable()
    {
        if (playerNetwork == null || playerNetwork.playerControls == null) return;

        playerNetwork.playerControls.Player.Look.performed -= OnLook;
        playerNetwork.playerControls.Player.Look.canceled -= OnLookCancel;

    }
}
