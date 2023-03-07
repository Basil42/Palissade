using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TowerBuilder : Singleton<TowerBuilder>
{
    private Node SelectedTile;
    Level grid;
    private Camera camRef;
    private Vector3 mousePos;
    [SerializeField] private Transform tileSelectionHighlighterTransform;

    public int towerNumber = 2;
    private int towerCounter = 0;

    [SerializeField] private GameObject towerPrefab;//TODO some kind of 3D tileset, good luck future me

    private void Awake()
    {
        camRef = Camera.main;
    
        GameManager.OnEnterTowerMode += () =>
        {
            this.enabled = true;
            Debug.Log("Tower mode");
        };
        GameManager.OnExitTowerMode += () =>
        {
            this.enabled = false;
            Debug.Log("Tower mode stop");
        };
    }

    private void Start()
    {
        grid = LevelManager.Instance.LevelRef;
    }

    private RaycastHit hit;
    private Vector2Int _tileCoord;

    void Update()
    {
        var selectedTile = TileSelector.SelectedTile;
        if (_tileCoord != TileSelector.SelectedTile?.Position && selectedTile != null)
        {
            _tileCoord = selectedTile.Position;
            tileSelectionHighlighterTransform.position = new Vector3( selectedTile.Position.x*grid.TileSize,0f,selectedTile.Position.y*grid.TileSize);

        }
        //confirm
        if (Input.GetMouseButtonDown(0))
        {
            //TODO: check all tile covered by the piece
            if (CanConstructWallOn(SelectedTile))
            {
                StopAllCoroutines();
                StartCoroutine(HighlighterErrorFeedback());
                return;
            }
            //TODO: apply the whole tetris piece after checking the relevant tiles for eligibility 

            Instantiate(towerPrefab, tileSelectionHighlighterTransform.position, quaternion.identity);
            SelectedTile.StateNode = EnumStateNode.tower;
            towerCounter++;
                //Debug.Log(towerCounter + " tour placée");

            if (towerCounter >= towerNumber)
            {
                towerCounter = 0;
                GameManager.Instance.NextMode();
            }
        }
    }

    private bool CanConstructWallOn(Node tile)
    {
        return tile.StateNode == EnumStateNode.water ||
               tile.StateNode == EnumStateNode.tower ||
               tile.StateNode == EnumStateNode.wall ||
               tile.StateNode == EnumStateNode.castle;
    }

    private IEnumerator HighlighterErrorFeedback()
    {
        //TODO: make the highlighter flash red
        yield break;
    }
}
