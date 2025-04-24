
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Trajectory/Straight")]
public class StraightTrajectory : TrajectoryStyle
{
    public override Vector3[] CalculateTrajectory(Vector3 startPos, Vector3 direction, float speed, float range)
    {
        int steps = Mathf.Max(2, Mathf.CeilToInt(range / 1f));
        Vector3[] positions = new Vector3[steps];
        direction.Normalize();

        for (int i = 0; i < steps; i++)
        {
            float distance = (i / (float)(steps - 1)) * range;
            positions[i] = startPos + direction * distance;
        }

        return positions;
    }


}


