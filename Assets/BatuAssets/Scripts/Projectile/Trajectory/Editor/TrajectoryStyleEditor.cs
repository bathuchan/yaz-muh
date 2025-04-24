using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrajectoryStyle), true)]
public class TrajectoryStyleEditor : Editor
{
    private TrajectoryStyle trajectory;

    private SerializedProperty previewStartPosProp;
    private SerializedProperty previewDirectionProp;
    private SerializedProperty previewSpeedProp;
    private SerializedProperty previewRangeProp;

    private void OnEnable()
    {
        trajectory = (TrajectoryStyle)target;
        SceneView.duringSceneGui += OnSceneGUI;

        previewStartPosProp = serializedObject.FindProperty("previewStartPosition");
        previewDirectionProp = serializedObject.FindProperty("previewDirection");
        previewSpeedProp = serializedObject.FindProperty("previewSpeed");
        previewRangeProp = serializedObject.FindProperty("previewRange");
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (trajectory == null)
            return;

        serializedObject.Update();

        Vector3 start = previewStartPosProp.vector3Value;
        Vector3 dir = previewDirectionProp.vector3Value.normalized;

        // Draw direction line
        Handles.color = Color.yellow;
        Handles.DrawLine(start, start + dir);

        // Draw trajectory
        Vector3[] points = trajectory.CalculateTrajectory(
            start,
            dir,
            previewSpeedProp.floatValue,
            previewRangeProp.floatValue
        );

        Handles.color = Color.cyan;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Handles.DrawLine(points[i], points[i + 1]);
        }

        // Draw start and end markers
        Handles.color = Color.green;
        Handles.SphereHandleCap(0, points[0], Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.color = Color.red;
        Handles.SphereHandleCap(0, points[^1], Quaternion.identity, 0.1f, EventType.Repaint);

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.HelpBox("Trajectory is previewed in the Scene view based on the preview values below.", MessageType.Info);

        if (GUILayout.Button("Repaint Scene View"))
        {
            SceneView.RepaintAll();
        }
    }
}
