using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using File = System.IO.File;

#if UNITY_EDITOR
public class TileTypeMapper : MonoBehaviour
{
    [SerializeField] private float seaLevel = 4.0f;
    private void OnGUI()
    {
        if (GUILayout.Button("Map"))
        {
            SerializeTileType();
        }

        if (GUILayout.Button("visualize"))
        {
            VisualizeMapTypes();
        }
    }

    [SerializeField] private RawImage visualizer; 
    private void VisualizeMapTypes()
    {
        var jsonString = File.ReadAllText(Application.dataPath + "/tileTypes.txt");
        var TypeMap = JsonConvert.DeserializeObject<EnumStateNode[][]>(jsonString);
        var mapDimension = new Vector2Int(LevelManager.Instance.LevelRef.Width, LevelManager.Instance.LevelRef.Height);
        Texture2D visTexture = new Texture2D(mapDimension.x, mapDimension.y);
        for (int x = 0; x < mapDimension.x; x++)
        {
            for (int y = 0; y < mapDimension.y; y++)
            {
                visTexture.SetPixel(x,y,TypeMap[x][y] == EnumStateNode.buildable ? Color.green : Color.blue);
            }
        }
        visTexture.Apply();
        if (visualizer == null)
        {
            Debug.LogWarning("No component to display the visualization");
            return;
        }

        visualizer.enabled = true;
        visualizer.texture = visTexture;
    }

    private void SerializeTileType()
    {
        var level = LevelManager.Instance.LevelRef;
        var stateArray = new EnumStateNode[level.Width][];
        for (int index = 0; index < level.Width; index++)
        {
            stateArray[index] = new EnumStateNode[level.Height];
        }

        RaycastHit hit;
        for (int x = 0; x < level.Width; x++)
        {
            for (int y = 0; y < level.Height; y++)
            {
                var castOrigin = level.GetCenterWorldPosition(level.Nodes[x, y].Position) + (Vector3.up * 20.0f);
                if (!Physics.Raycast(castOrigin,
                        Vector3.down,
                        out hit))
                {
                    Debug.LogError("missed the ground, aborting");
                    return;
                }

                stateArray[x][y] = hit.point.y > seaLevel ? EnumStateNode.buildable : EnumStateNode.water;
            }

            string jsonArray = JsonConvert.SerializeObject(stateArray);
            File.WriteAllText(Application.dataPath + "/tileTypes.txt",jsonArray);
        }
    }
}
#endif