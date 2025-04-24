using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Trajectory/CurveBased")]

public class CurveBasedTrajectory : TrajectoryStyle
{


    [Tooltip("Single curve used to define offset along the perpendicular X axis over time.")]
    public AnimationCurve lateralCurve;

    [Tooltip("Distance to move forward in the direction over time (normalized 0 to 1).")]
    public float forwardDistance = 1f;

    public int pointCount = 50;

    public override Vector3[] CalculateTrajectory(Vector3 startPosition, Vector3 direction, float speed, float range)
    {
        Vector3[] points = new Vector3[pointCount];
        direction.Normalize();

        // Find a perpendicular vector in the XZ plane
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);

            float forwardStep = t * forwardDistance * range;
            float lateralOffset = lateralCurve.Evaluate(t) * range;

            Vector3 offset = direction * forwardStep + right * lateralOffset;
            points[i] = startPosition + offset;
        }

        return points;
    }


}
