using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class InitialPhaseController : Singleton<InitialPhaseController>
{
    public Node SelectedCastle { get; private set; } = null;
    private Level _levelRef;

    private void Start()
    {
        _levelRef = LevelManager.Instance.LevelRef;
    }

    internal IEnumerator CastleSelection()
    {
        while (SelectedCastle == null)
        {
            var selectedTile = TileSelector.SelectedTile;

            if (Input.GetMouseButtonDown(0))
            {
                if (selectedTile is { StateNode: EnumStateNode.castle })
                {
                    SelectedCastle = selectedTile;
                    Debug.Log("selected Castle");
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
        var castleCoord = SelectedCastle.Position;
        //upper left corner
        
        Instantiate(
            cornerStartingWallPiece,
            new Vector3((SelectedCastle.Position.x - initialZoneOfControlExtent.x) * _levelRef.TileSize,0f,(SelectedCastle.Position.y + initialZoneOfControlExtent.y) * _levelRef.TileSize),
            quaternion.Euler(0f,0.5f * Mathf.PI * cornerRotationOffset,0f));
        yield return waiter;
        //upper path
        var startCoord = castleCoord.x - (initialZoneOfControlExtent.x - 1);
        var endCoord = castleCoord.x + (initialZoneOfControlExtent.x - 1);
        var staticCoordValue = SelectedCastle.Position.y + initialZoneOfControlExtent.y;
        for (int x = startCoord; x <= endCoord; x++)
        {
            Instantiate(pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(new Vector2Int(x, staticCoordValue)),
                    quaternion.Euler(0f, 0.5f * Mathf.PI  * pathRotationOffset, 0f));
            yield return waiter;
        }
        //upper right corner
        Instantiate(
            cornerStartingWallPiece,
            _levelRef.GetCenterWorldPosition(SelectedCastle.Position.x + initialZoneOfControlExtent.x,staticCoordValue),
            quaternion.Euler(0f,0.5f * Mathf.PI  * (1 +cornerRotationOffset),0f));
        yield return waiter;
        //right path
        startCoord = castleCoord.y + (initialZoneOfControlExtent.y-1);
        endCoord = castleCoord.y - (initialZoneOfControlExtent.y-1);
        staticCoordValue = SelectedCastle.Position.x + initialZoneOfControlExtent.x;
        for (int y = startCoord; y >= endCoord; y--)
        {
            Instantiate(pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(new Vector2Int(staticCoordValue, y)),
                quaternion.Euler(0f, 0.5f * Mathf.PI  * (pathRotationOffset + 1), 0f));
            yield return waiter;
        }
        //lower right corner
        Instantiate(
            cornerStartingWallPiece,
            _levelRef.GetCenterWorldPosition(SelectedCastle.Position.x + initialZoneOfControlExtent.x,
                SelectedCastle.Position.y - initialZoneOfControlExtent.y),
            quaternion.Euler(0f, 0.5f * Mathf.PI  * (2 + cornerRotationOffset), 0f));
        yield return waiter;
        //lower path
        startCoord = castleCoord.x + (initialZoneOfControlExtent.x - 1);
        endCoord = castleCoord.x - (initialZoneOfControlExtent.x - 1);
        staticCoordValue = SelectedCastle.Position.y - initialZoneOfControlExtent.y;
        for (int x = startCoord; x >= endCoord; x--)
        {
            Instantiate(
                pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(x, staticCoordValue),
                quaternion.Euler(0f, 0.5f * Mathf.PI  * (pathRotationOffset), 0f));
            yield return waiter;
        }
        //lower left corner
        Instantiate(cornerStartingWallPiece,
            _levelRef.GetCenterWorldPosition(castleCoord - initialZoneOfControlExtent),
            quaternion.Euler(0f, 0.5f * Mathf.PI  * (3 + cornerRotationOffset), 0f));
        yield return waiter;
        //left path
        startCoord = castleCoord.y - (initialZoneOfControlExtent.y - 1);
        endCoord = castleCoord.y + (initialZoneOfControlExtent.y - 1);
        staticCoordValue = castleCoord.x - initialZoneOfControlExtent.x;
        for (int y = startCoord; y <= endCoord; y++)
        {
            Instantiate(pathStartingWallPiece,
                _levelRef.GetCenterWorldPosition(staticCoordValue, y),
                quaternion.Euler(0f, 0.5f * Mathf.PI  * (1+ pathRotationOffset), 0f));
            yield return waiter;
        }
    }

    internal void InitialZoneOfControl()
    {
        ZoneOfControl.Instance.CheckRampartAreValid(SelectedCastle);
    }
    
    
}
