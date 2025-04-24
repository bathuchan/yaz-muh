using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryManager : NetworkBehaviour
{
    private LineRenderer lineRenderer;
    public Transform trajectoryStartPosition;
    public Transform currentAimPosition;
    public PlayerLook playerLook;
    public PlayerAbility playerAbility;

    private Coroutine trajectoryRoutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        trajectoryStartPosition = this.transform;
        playerLook = GetComponentInParent<PlayerLook>();
        playerAbility = GetComponentInParent<PlayerAbility>();
    }

    // Starts the coroutine-based trajectory drawing
    public void StartTrajectory()
    {
        if (!IsOwner) return;  // Ensure only the owning player starts the trajectory drawing

        if (trajectoryRoutine != null) return;
        trajectoryRoutine = StartCoroutine(UpdateTrajectoryRoutine());
    }

    // Stops the coroutine and clears the line
    public void StopTrajectory()
    {
        if (!IsOwner) return;  // Ensure only the owning player stops the trajectory drawing

        if (trajectoryRoutine != null)
        {
            StopCoroutine(trajectoryRoutine);
            trajectoryRoutine = null;
        }

        ClearTrajectory();
    }

    private IEnumerator UpdateTrajectoryRoutine()
    {
        while (true)
        {
            if (playerAbility.currentProjectileData != null && playerAbility.currentProjectileData.trajectoryStyle != null)
            {
                ShowTrajectory(trajectoryStartPosition.position, playerLook.playerModel.transform.forward, playerAbility.currentProjectileData);
            }

            yield return null;
        }
    }

    public void ShowTrajectory(Vector3 startPos, Vector3 direction, ProjectileData projectileData)
    {
        if (!IsOwner) return;

        if (projectileData == null || projectileData.trajectoryStyle == null) return;

        // Determine the actual starting point
        Vector3[] fullPath;

        if (projectileData.trajectoryStyle is CircularTrajectory)
        {
            fullPath = projectileData.trajectoryStyle.CalculateTrajectory(currentAimPosition.position, direction, projectileData.speed, projectileData.range);
        }
        else
        {
            fullPath = projectileData.trajectoryStyle.CalculateTrajectory(startPos, direction, projectileData.speed, projectileData.range);
        }

        // No raycast check for circular paths, just draw the full shape
        if (projectileData.trajectoryStyle is CircularTrajectory)
        {
            lineRenderer.positionCount = fullPath.Length;
            lineRenderer.loop = true; // Make sure it's circular
            lineRenderer.SetPositions(fullPath);
            return;
        }

        // Normal trajectory with raycast collision checks
        List<Vector3> visiblePath = new List<Vector3> { fullPath[0] };

        for (int i = 1; i < fullPath.Length; i++)
        {
            Vector3 from = fullPath[i - 1];
            Vector3 to = fullPath[i];
            Vector3 rayDir = to - from;
            float rayDistance = rayDir.magnitude;

            if (Physics.Raycast(from, rayDir.normalized, out RaycastHit hit, rayDistance))
            {
                visiblePath.Add(hit.point);
                break;
            }
            else
            {
                visiblePath.Add(to);
            }
        }

        lineRenderer.loop = false;
        lineRenderer.positionCount = visiblePath.Count;
        lineRenderer.SetPositions(visiblePath.ToArray());
    }
    public void ClearTrajectory()
    {
        if (!IsOwner) return;  // Ensure only the owning player clears the trajectory

        lineRenderer.positionCount = 0;
    }
}
