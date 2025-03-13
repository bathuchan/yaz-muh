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

    private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>(
       new PlayerData(100, 50),
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server
   );

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log("Player id:"+OwnerClientId + " - randomNumber: " + randomNumber.Value);
        };
        playerData.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"Player id:{OwnerClientId} Health: {newValue.playerHealth}, Shield: {newValue.playerShield}");
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamageServerRpc(5);
        }

        Vector3 movement = new Vector3(moveDir.x, 0, moveDir.y) * moveSpeed * Time.fixedDeltaTime;
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
        // Apply movement on the server using the stored Rigidbody reference
        rb.MovePosition(rb.position + movement);

        // Send updated position back to clients
        UpdateClientPositionClientRpc(rb.position);
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
    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        PlayerData data = playerData.Value;
        data.playerHealth -= damage;
        playerData.Value = data;
    }
}
