using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Trajectory/Circular")]
public class CircularTrajectory : TrajectoryStyle
{
    public int pointCount = 36;
    public float radius = 3f;

    public override Vector3[] CalculateTrajectory(Vector3 centerPosition, Vector3 direction, float speed, float range)
    {
        Vector3[] points = new Vector3[pointCount + 1];

        for (int i = 0; i <= pointCount; i++)
        {
            float angle = (float)i / pointCount * Mathf.PI * 2;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            points[i] = centerPosition + offset;
        }

        return points;
    }
}