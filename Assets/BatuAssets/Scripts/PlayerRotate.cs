using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotate : NetworkBehaviour
{
   

    private NetworkVariable<Quaternion> syncedRotation = new NetworkVariable<Quaternion>(
        Quaternion.identity,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );


   

    private void Update()
    {
        //if (IsOwner)
        //{
        //    RotatePlayer();
        //    //Debug.DrawRay(transform.position, transform.forward, Color.green);
        //}
        //else
        //{
        //    // Non-owner clients smoothly interpolate towards the synced rotation
        //    playerModel.rotation = Quaternion.Slerp(playerModel.rotation, syncedRotation.Value, Time.deltaTime * rotationSpeed);
        //}
    }
    //private void RotatePlayer()
    //{
    //    Vector3 targetDirection = Vector3.zero;

    //    if (lookInput.sqrMagnitude > 0.01f)
    //    {
    //        targetDirection = lookInput;
    //    }
    //    else if (playerNetwork.moveDir.sqrMagnitude > 0.01f)
    //    {
    //        targetDirection = playerNetwork.moveDir;
    //    }

    //    if (targetDirection.sqrMagnitude > 0.01f)
    //    {
    //        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
    //        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
    //        playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    //        // Send rotation to the server
    //        SendRotationToServerRpc(playerModel.rotation);
    //    }
    //}

    //[ServerRpc]
    //private void SendRotationToServerRpc(Quaternion rotation)
    //{
    //    syncedRotation.Value = rotation;
    //}

}
