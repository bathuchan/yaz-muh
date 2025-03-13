using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : NetworkBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Only assign camera for the local player
        {
            FindAndAssignCamera();
        }
    }

    private void FindAndAssignCamera()
    {
        // Find an existing Cinemachine Virtual Camera in the scene
        cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineCamera == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found in the scene!");
            return;
        }

        // Set the virtual camera to follow and look at the player
        cinemachineCamera.Follow = transform;
        cinemachineCamera.LookAt = transform;
    }
}
