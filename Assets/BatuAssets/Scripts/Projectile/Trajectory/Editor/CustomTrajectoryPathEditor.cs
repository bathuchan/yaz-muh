using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomTrajectoryPath))]
public class CustomTrajectoryPathEditor : Editor
{
    private CustomTrajectoryPath customPath;

    private void OnEnable()
    {
        customPath = (CustomTrajectoryPath)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (customPath == null || customPath.localPathPoints == null) return;

        Handles.color = Color.green;
        Vector3 previewStart = customPath.previewStartPosition;
        Quaternion rotation = Quaternion.LookRotation(customPath.previewDirection.normalized);

        for (int i = 0; i < customPath.localPathPoints.Count; i++)
        {
            Vector3 localPoint = customPath.localPathPoints[i];
            Vector3 worldPoint = previewStart + rotation * localPoint;

            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPoint = Handles.PositionHandle(worldPoint, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(customPath, "Move Path Point");
                customPath.localPathPoints[i] = Quaternion.Inverse(rotation) * (newWorldPoint - previewStart);
                EditorUtility.SetDirty(customPath);
            }

            Handles.Label(worldPoint + Vector3.up * 0.1f, $"Point {i}");
        }

        Handles.color = Color.cyan;
        for (int i = 0; i < customPath.localPathPoints.Count - 1; i++)
        {
            Vector3 a = previewStart + rotation * customPath.localPathPoints[i];
            Vector3 b = previewStart + rotation * customPath.localPathPoints[i + 1];
            Handles.DrawLine(a, b);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(customPath.applySmoothing)));

        if (customPath.applySmoothing)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(customPath.smoothingStep)));

            // Estimate and display the dynamic max point count
            int smoothingCount = (customPath.localPathPoints.Count - 3) * Mathf.RoundToInt(1f / Mathf.Max(0.01f, customPath.smoothingStep));
            smoothingCount = Mathf.Max(smoothingCount, customPath.localPathPoints.Count);

            EditorGUILayout.HelpBox($"Smoothing enabled. Estimated point count: {smoothingCount}", MessageType.Info);
            customPath.maxPointCount = smoothingCount;
        }
        else
        {
            SerializedProperty maxPointsProp = serializedObject.FindProperty(nameof(customPath.maxPointCount));
            EditorGUILayout.PropertyField(maxPointsProp);
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(customPath.localPathPoints)), true);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(customPath.speed)));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(customPath.stepSize)));

        EditorGUILayout.Space();
        if (GUILayout.Button("Add Point"))
        {
            Undo.RecordObject(customPath, "Add Point");
            Vector3 lastPoint = customPath.localPathPoints.Count > 0 ?
                customPath.localPathPoints[customPath.localPathPoints.Count - 1] :
                Vector3.forward;

            customPath.localPathPoints.Add(lastPoint + Vector3.forward);
            EditorUtility.SetDirty(customPath);
        }

        if (GUILayout.Button("Clear Points"))
        {
            Undo.RecordObject(customPath, "Clear Points");
            customPath.localPathPoints.Clear();
            EditorUtility.SetDirty(customPath);
        }

        serializedObject.ApplyModifiedProperties();
    }
}