using UnityEngine;

public abstract class TrajectoryStyle : ScriptableObject
{
    public abstract Vector3[] CalculateTrajectory(Vector3 startPosition, Vector3 direction, float speed, float range);

#if UNITY_EDITOR
    [Header("Editor Preview")]
    public Vector3 previewStartPosition = Vector3.zero;
    public Vector3 previewDirection = Vector3.right;
    public float previewSpeed = 10f;
    public float previewRange = 10f;
#endif
}
