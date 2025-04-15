using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Trajectory/Zig-Zag")]
public class ZigZagTrajectory : TrajectoryStyle
{
    public int pointCount = 50;
    public float amplitude = 0.5f;
    public float frequency = 5f;


    public override Vector3[] CalculateTrajectory(Vector3 startPos, Vector3 direction, float speed, float range)
    {
        Vector3[] points = new Vector3[pointCount];
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            float forwardDist = t * range;
            float offset = Mathf.Sin(t * Mathf.PI * frequency) * amplitude;

            Vector3 zigzagOffset = right * offset;
            points[i] = startPos + direction.normalized * forwardDist + zigzagOffset;
        }

        return points;
    }
}
