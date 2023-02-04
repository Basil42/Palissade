using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestCameraMask))]
public class CameraTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestCameraMask cam = target as TestCameraMask;
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            cam.ChangeMode();
        }
    }
}
