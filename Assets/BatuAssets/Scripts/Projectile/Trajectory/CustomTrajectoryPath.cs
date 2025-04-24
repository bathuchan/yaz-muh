using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/Trajectory/CustomPath")]
public class CustomTrajectoryPath : TrajectoryStyle
{
    public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }

    public List<Vector3> localPathPoints = new List<Vector3>();
    public bool applySmoothing = true;

    [HideInInspector] public int maxPointCount = 100;
    [Range(0.01f, 1f)] public float smoothingStep = 0.1f;

    //public float speed = 10f;
    //public float stepSize = 1f;

    public override Vector3[] CalculateTrajectory(Vector3 startPosition, Vector3 direction, float speed, float range)
    {
        direction.Normalize();
        Quaternion rotation = Quaternion.LookRotation(direction);

        List<Vector3> worldPoints = new List<Vector3>();
        foreach (var localPoint in localPathPoints)
        {
            worldPoints.Add(startPosition + rotation * localPoint);
        }

        if (applySmoothing)
        {
            // Calculate max point count dynamically based on smooth step
            int dynamicCount = Mathf.Clamp((localPathPoints.Count - 3) * Mathf.CeilToInt(1f / smoothingStep), 10, 500);
            maxPointCount = dynamicCount;
        }

        float totalDistance = 0f;
        for (int i = 0; i < worldPoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(worldPoints[i], worldPoints[i + 1]);
        }

        List<Vector3> resampledPoints = ResamplePoints(worldPoints, totalDistance, maxPointCount);

        if (applySmoothing && resampledPoints.Count > 3)
        {
            resampledPoints = ApplySmoothing(resampledPoints);
        }

        return resampledPoints.ToArray();
    }
    private List<Vector3> ResampleEvenly(List<Vector3> inputPoints, int finalPointCount)
    {
        if (inputPoints.Count < 2)
            return inputPoints;

        float totalLength = 0f;
        List<float> segmentLengths = new List<float>();
        for (int i = 0; i < inputPoints.Count - 1; i++)
        {
            float dist = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);
            segmentLengths.Add(dist);
            totalLength += dist;
        }

        float interval = totalLength / (finalPointCount - 1);
        List<Vector3> result = new List<Vector3> { inputPoints[0] };

        int segment = 0;
        float distSoFar = 0f;

        for (int i = 1; i < finalPointCount - 1; i++)
        {
            float targetDist = i * interval;

            while (segment < segmentLengths.Count && distSoFar + segmentLengths[segment] < targetDist)
            {
                distSoFar += segmentLengths[segment];
                segment++;
            }

            if (segment >= segmentLengths.Count)
                break;

            float t = (targetDist - distSoFar) / segmentLengths[segment];
            Vector3 point = Vector3.Lerp(inputPoints[segment], inputPoints[segment + 1], t);
            result.Add(point);
        }

        result.Add(inputPoints[inputPoints.Count - 1]); // Ensure last point is included
        return result;
    }


    private List<Vector3> ResamplePoints(List<Vector3> worldPoints, float totalDistance, int maxPointCount)
    {
        float adjustedStepSize = totalDistance / (maxPointCount - 1);
        List<Vector3> resampledPoints = new List<Vector3> { worldPoints[0] };

        float currentDistance = 0f;
        int currentSegment = 0;

        for (int i = 1; i < maxPointCount; i++)
        {
            float targetDistance = i * adjustedStepSize;

            while (currentSegment < worldPoints.Count - 1 &&
                   currentDistance + Vector3.Distance(worldPoints[currentSegment], worldPoints[currentSegment + 1]) < targetDistance)
            {
                currentDistance += Vector3.Distance(worldPoints[currentSegment], worldPoints[currentSegment + 1]);
                currentSegment++;
            }

            if (currentSegment >= worldPoints.Count - 1)
                break;

            float segmentLength = Vector3.Distance(worldPoints[currentSegment], worldPoints[currentSegment + 1]);
            float remaining = targetDistance - currentDistance;
            float t = Mathf.Clamp01(remaining / segmentLength);

            Vector3 point = Vector3.Lerp(worldPoints[currentSegment], worldPoints[currentSegment + 1], t);
            resampledPoints.Add(point);
        }

        resampledPoints.Add(worldPoints[worldPoints.Count - 1]);
        return resampledPoints;
    }

    private List<Vector3> ApplySmoothing(List<Vector3> resampledPoints)
    {
        List<Vector3> smoothedPoints = new List<Vector3>();

        if (resampledPoints.Count < 4)
            return resampledPoints;

        List<Vector3> padded = new List<Vector3>
    {
        resampledPoints[0],
        resampledPoints[0]
    };
        padded.AddRange(resampledPoints);
        padded.Add(resampledPoints[resampledPoints.Count - 1]);
        padded.Add(resampledPoints[resampledPoints.Count - 1]);

        for (int i = 2; i < padded.Count - 3; i++)
        {
            Vector3 p0 = padded[i - 2];
            Vector3 p1 = padded[i - 1];
            Vector3 p2 = padded[i];
            Vector3 p3 = padded[i + 1];

            if (ArePointsCollinear(p0, p1, p2, p3))
            {
                smoothedPoints.Add(p1);
            }
            else
            {
                for (float t = 0f; t < 1f; t += 0.1f)
                {
                    smoothedPoints.Add(CatmullRom(p0, p1, p2, p3, t));
                }
            }
        }

        smoothedPoints.Add(resampledPoints[resampledPoints.Count - 1]);

        // Resample again evenly to reduce clutter
        return ResampleEvenly(smoothedPoints, maxPointCount);
    }

    private bool ArePointsCollinear(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 v1 = p1 - p0;
        Vector3 v2 = p2 - p1;
        Vector3 v3 = p3 - p2;

        return Vector3.Cross(v1, v2).sqrMagnitude < 0.0001f && Vector3.Cross(v2, v3).sqrMagnitude < 0.0001f;
    }
}
