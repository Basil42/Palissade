using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : Singleton<TileSelector>
{
    private Node _selectedTile;

    public static Node SelectedTile => Instance._selectedTile;

    private Vector3 _mousePos;

    private Ray _mouseRay;

    private RaycastHit _hit;

    private Camera _camRef;

    private void Awake()
    {
        _camRef = Camera.main;
        #if UNITY_EDITOR
        _style = new GUIStyle()
        {
            fontSize = 32
        };
#endif
    }

    // Update is called once per frame
    void Update()
    {
        _mousePos = Input.mousePosition;
        _mouseRay = _camRef.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(_mouseRay, out _hit))
        {
            var levelRef = LevelManager.Instance.LevelRef;
            float tileSize = levelRef.TileSize;
            Vector2Int tileCoordinates = new Vector2Int(
                Mathf.FloorToInt(_hit.point.x / tileSize),
                Mathf.FloorToInt(_hit.point.z / tileSize));
            tileCoordinates.Clamp(Vector2Int.zero,new Vector2Int(levelRef.Width,levelRef.Height));
            if (tileCoordinates != _selectedTile?.Position)
            {
                try
                {
                    _selectedTile = levelRef.Nodes[tileCoordinates.x, tileCoordinates.y];

                }
                catch (System.IndexOutOfRangeException e)
                {
                    Console.WriteLine(e + $"index was {tileCoordinates}");
                    throw;
                }
            }
            
        }
    }

    #if UNITY_EDITOR
    private GUIStyle _style;
    private void OnGUI()
    {
        
        GUILayout.Label($"{_selectedTile.Position}",_style);
    }
    #endif
}
