using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelRenderer))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelRenderer level = target as LevelRenderer;
        EditorGUILayout.Space();



        if(GUILayout.Button("DestroyAll"))
        {
            level.DestroyAllChilds();
        }
    }
}
