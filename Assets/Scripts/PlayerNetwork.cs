using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent (typeof (Rigidbody))]
public class PlayerNetwork : NetworkBehaviour
{
    private PlayerControls playerControls;
    private float moveSpeed = 3f;
    private Vector3 moveDir;
    private Rigidbody rb;

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log("Player id:"+OwnerClientId + " - randomNumber: " + randomNumber.Value);
        };
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMoveCancel;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        moveDir = Vector3.zero;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = Random.Range(0, 100);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector3 movement = new Vector3(moveDir.x, 0, moveDir.y) * moveSpeed * Time.fixedDeltaTime;

        // Send movement to the server
        SendInputToServerRpc(movement);
    }

    [ServerRpc]
    private void SendInputToServerRpc(Vector3 movement)
    {
        // Apply movement on the server using Rigidbody
        Rigidbody serverRb = GetComponent<Rigidbody>();
        serverRb.MovePosition(serverRb.position + movement);

        // Send the updated position back to clients
        UpdateClientPositionClientRpc(serverRb.position);
    }

    [ClientRpc]
    private void UpdateClientPositionClientRpc(Vector3 newPosition)
    {
        if (!IsOwner)
        {
            // Interpolate towards the server position for smooth movement
            //StartCoroutine(InterpolateToPosition(newPosition));
        }
    }

    private System.Collections.IEnumerator InterpolateToPosition(Vector3 targetPos)
    {
        float elapsedTime = 0f;
        float lerpDuration = 0.1f; // Adjust for smoothness

        Vector3 startPos = rb.position;

        while (elapsedTime < lerpDuration)
        {
            rb.position = Vector3.Lerp(startPos, targetPos, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.position = targetPos;
    }
}
