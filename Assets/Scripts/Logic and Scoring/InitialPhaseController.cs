using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitialPhaseController : MonoBehaviour
{
    internal IEnumerator CastleSelection()
    {
        Node selectedCastle = null;
        Vector3 mousePos;
        Ray MouseRay;
        RaycastHit hit;
        Camera camRef = Camera.main;
        Node SelectedTile = null;
        while (selectedCastle == null)
        {
            mousePos = Input.mousePosition;
            MouseRay = camRef.ScreenPointToRay(mousePos);
            if (Physics.Raycast(MouseRay, out hit))
            {
                var level = LevelManager.Instance.LevelRef;
                float tileSize = LevelManager.Instance.LevelRef.TileSize;
                Vector2Int tileCoordinates = new Vector2Int(Mathf.FloorToInt(hit.point.x / tileSize),
                    Mathf.FloorToInt(hit.point.y / tileSize));
                if (tileCoordinates != SelectedTile?.Position)
                {
                    SelectedTile = level.Nodes[tileCoordinates.x, tileCoordinates.y];
                    //TODO: highlight castle selection
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (SelectedTile is { StateNode: EnumStateNode.castle })
                {
                    selectedCastle = SelectedTile;
                }
                else
                {
                    Debug.Log("Not a castle tile.");
                    //TODO: visual error feedback (like a red flash on the tile or something)
                }
            }
            yield return null;
        }
        
    }

    [SerializeField] private Vector2Int initialZoneOfControlSize = new Vector2Int(6,5);
    internal IEnumerator InitialWallBuilding()
    {
        
        //upper corner
        yield return null;
        
    }

    internal IEnumerator InitialZoneOfControl()
    {
        //wait on an animation for the zone of control, make this a void method if the animation should be waited on in the game manager
        //call on external zone of control method
        yield return null; //wait on animation
    }
    
    
}
