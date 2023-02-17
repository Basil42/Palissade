using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class InitialPhaseController : MonoBehaviour
{
    Node _selectedCastle = null;
    private Level _levelRef;
    internal IEnumerator CastleSelection()
    {
        while (_selectedCastle == null)
        {
            var selectedTile = TileSelector.SelectedTile;

            if (Input.GetMouseButtonDown(0))
            {
                if (selectedTile is { StateNode: EnumStateNode.castle })
                {
                    _selectedCastle = selectedTile;
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

    [Tooltip("How much the initial wall extend from the castle")][SerializeField] private Vector2Int initialZoneOfControlExtent = new Vector2Int(4,3);
    [Tooltip("number of quarter clock wise turn the corner prefab has compared to the upper left corner position.")]
    [SerializeField] private int cornerRotationOffset = 0;
    [Tooltip("number of quarter clock wise turn of the path prefab (should be 0 or 1)")] 
    [SerializeField] private int pathRotationOffset = 0;
    [Header("wall prefabs")]
    [SerializeField] private GameObject cornerStartingWallPiece;
    [SerializeField] private GameObject pathStartingWallPiece;
    [Header("animation")] [SerializeField] private float wallBuildingTimeStep = 0.5f;
    internal IEnumerator InitialWallBuilding()
    {
        //i should just have used the builder and run the tileset rules automatically
        var waiter = new WaitForSeconds(wallBuildingTimeStep);
        var castleCoord = _selectedCastle.Position;
        //upper left corner
        Instantiate(
            cornerStartingWallPiece,
            new Vector3((_selectedCastle.Position.x - initialZoneOfControlExtent.x) * _levelRef.TileSize,0f,(_selectedCastle.Position.y + initialZoneOfControlExtent.y) * _levelRef.TileSize),
            quaternion.Euler(0f,90f* cornerRotationOffset,0f));
        yield return waiter;
        //upper path
        var startCoord = castleCoord.x - (initialZoneOfControlExtent.x - 1);
        var endCoord = castleCoord.x + (initialZoneOfControlExtent.x - 1);
        var staticCoordValue = _selectedCastle.Position.y + initialZoneOfControlExtent.y;
        for (int x = startCoord; x < endCoord; x++)
        {
            Instantiate(pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(new Vector2Int(x, staticCoordValue)),
                    Quaternion.Euler(0f, 90f * pathRotationOffset, 0f));
            yield return waiter;
        }
        //upper right corner
        Instantiate(
            cornerStartingWallPiece,
            _levelRef.GetCenterWorldPosition(_selectedCastle.Position.x + initialZoneOfControlExtent.x,staticCoordValue),
            quaternion.Euler(0f,90f * (1 +cornerRotationOffset),0f));
        yield return waiter;
        //right path
        startCoord = castleCoord.y + (initialZoneOfControlExtent.y-1);
        endCoord = castleCoord.y - (initialZoneOfControlExtent.y-1);
        staticCoordValue = _selectedCastle.Position.x + initialZoneOfControlExtent.x;
        for (int y = startCoord; y < endCoord; y++)
        {
            Instantiate(pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(new Vector2Int(staticCoordValue, y)),
                Quaternion.Euler(0f, 90f * (pathRotationOffset + 1), 0f));
            yield return waiter;
        }
        //lower right corner
        Instantiate(
            cornerStartingWallPiece,
            _levelRef.GetCenterWorldPosition(_selectedCastle.Position.x + initialZoneOfControlExtent.x,
                _selectedCastle.Position.y - initialZoneOfControlExtent.y),
            Quaternion.Euler(0f, 90f * (2 + cornerRotationOffset), 0f));
        yield return waiter;
        //lower path
        startCoord = castleCoord.x + (initialZoneOfControlExtent.x - 1);
        endCoord = castleCoord.x - (initialZoneOfControlExtent.x - 1);
        staticCoordValue = _selectedCastle.Position.y - initialZoneOfControlExtent.y;
        for (int x = startCoord; x < endCoord; x++)
        {
            Instantiate(
                pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(x, staticCoordValue),
                Quaternion.Euler(0f, 90f * (pathRotationOffset), 0f));
            yield return waiter;
        }
        //lower left corner
        Instantiate(cornerStartingWallPiece,
            _levelRef.GetCenterWorldPosition(castleCoord - initialZoneOfControlExtent),
            Quaternion.Euler(0f, 90f * (3 * cornerRotationOffset), 0f));
        yield return waiter;
        //left path
        startCoord = castleCoord.y - (initialZoneOfControlExtent.y - 1);
        endCoord = castleCoord.y + (initialZoneOfControlExtent.y - 1);
        staticCoordValue = castleCoord.x - initialZoneOfControlExtent.x;
        for (int y = startCoord; y < endCoord; y++)
        {
            Instantiate(pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(staticCoordValue, y),
                Quaternion.Euler(0f, 90f * pathRotationOffset, 0f));
            yield return waiter;
        }
    }

    internal IEnumerator InitialZoneOfControl()
    {
        //wait on an animation for the zone of control, make this a void method if the animation should be waited on in the game manager
        //call on external zone of control method
        yield return null; //wait on animation
    }
    
    
}
