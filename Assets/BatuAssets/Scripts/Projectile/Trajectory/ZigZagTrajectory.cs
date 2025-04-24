using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Trajectory/Zig-Zag")]
public class ZigZagTrajectory : TrajectoryStyle
{
    public int pointCount = 50;
    public float amplitude = 0.5f;
    public float frequency = 5f;
    public ZigZagStyle zigZagMode = ZigZagStyle.SmoothWave;

    public override Vector3[] CalculateTrajectory(Vector3 startPosition, Vector3 direction, float speed, float range)
    {
        Vector3[] points = new Vector3[pointCount];
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            float forwardDist = t * range;

            float offset = 0f;

            switch (zigZagMode)
            {
                case ZigZagStyle.SmoothWave:
                    offset = Mathf.Sin(t * Mathf.PI * frequency) * amplitude;
                    break;

                case ZigZagStyle.SharpTriangle:
                    float waveT = t * frequency + 0.25f; // phase shift to start at 0 offset
                    offset = (4f * amplitude * Mathf.Abs((waveT % 1f) - 0.5f)) - amplitude;
                    break;
            }

            Vector3 zigzagOffset = right * offset;
            points[i] = startPosition + direction.normalized * forwardDist + zigzagOffset;
        }

        return points;
    }
}

public enum ZigZagStyle
{
    SmoothWave,
    SharpTriangle
}
