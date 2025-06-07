using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawnPoint : MonoBehaviour
{
    public Color gizmoColor = Color.green;
    public float gizmoRadius = 0.4f;
    public float arrowLength = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Draw a sphere at the spawn point position
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);

        // Draw a forward arrow
        Vector3 forward = transform.forward * arrowLength;
        Gizmos.DrawLine(transform.position, transform.position + forward);

#if UNITY_EDITOR
        // Draw arrowhead using Handles
        Handles.color = gizmoColor;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.forward), arrowLength, EventType.Repaint);

        // Draw object name label in scene view
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = gizmoColor;
        labelStyle.fontStyle = FontStyle.Bold;

        Handles.Label(transform.position + Vector3.up * (gizmoRadius + 0.1f), gameObject.name, labelStyle);
#endif
    }
}
