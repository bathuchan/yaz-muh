
using UnityEngine;

public abstract class TrajectoryStyle : ScriptableObject
{
    public abstract Vector3[] CalculateTrajectory(Vector3 startPos, Vector3 direction, float speed, float range);
}


