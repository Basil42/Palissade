using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager _gameManager = target as GameManager;
        EditorGUILayout.Space();

        if (GUILayout.Button("ChangeGraphicMode"))
        {
            _gameManager.ChangeGraphicMode();
        }

        if(GUILayout.Button("NextMode"))
        {
            _gameManager.NextMode();
        }
    }
}
