using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    public PlayerControls playerControls;
    
    public Vector3 moveDir { get; private set; }
    public Rigidbody playerRb;


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
            Debug.Log("Player id:" + OwnerClientId + " - randomNumber: " + randomNumber.Value);
        };
        playerData.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"Player id:{OwnerClientId} Health: {newValue.currentHealth}, Shield: {newValue.shield}");
        };

        if (!IsOwner) return;

        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMoveCancel;
        playerControls.Enable();


    }


    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();

        playerControls = new PlayerControls();
    }
    

    private void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        moveDir = Vector3.zero;
    }

    private void OnDisable()//remove listeners
    {
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMoveCancel;
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

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector3 movement = new Vector3(moveDir.x, 0, moveDir.y) * playerData.Value.movementSpeed * Time.fixedDeltaTime;
        SendInputToServerRpc(movement);
    }

    [ServerRpc]
    private void SendInputToServerRpc(Vector3 movement)
    {
        // Apply movement on the server using the stored Rigidbody reference
        playerRb.MovePosition(playerRb.position + movement);

    }

    [ClientRpc]//tried client prediction
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
        //corotine approach is not the way(costy on lient side)
        float elapsedTime = 0f;
        float lerpDuration = 0.1f; 

        Vector3 startPos = playerRb.position;

        while (elapsedTime < lerpDuration)
        {
            playerRb.position = Vector3.Lerp(startPos, targetPos, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerRb.position = targetPos;
    }
    [ServerRpc]//test function to register damage
    public void TakeDamageServerRpc(int damage)
    {
        PlayerData data = playerData.Value;
        data.currentHealth -= damage;
        playerData.Value = data;
    }

}
