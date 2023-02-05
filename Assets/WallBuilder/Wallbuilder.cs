using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Wallbuilder : MonoBehaviour
{
    private Node SelectedTile;
    [SerializeField] Level grid;
    private Vector3 mousePos;
    private Camera camRef;
    [SerializeField] private Transform tileSelectionHighlighterTransform;
    [SerializeField] private GameObject wallPrefab;//TODO some kind of 3D tileset, good luck future me
    private void Awake()
    {
        camRef = Camera.main;
    }

    private RaycastHit hit;
    private Vector2Int tileCoord;
    void Update()
    {
        mousePos = Input.mousePosition;
        Ray mouseRay = camRef.ScreenPointToRay(mousePos);
        //selection
        if (Physics.Raycast(mouseRay, out hit))
        {
            
            tileCoord = new Vector2Int(Mathf.FloorToInt(hit.point.x/grid.TileSize),Mathf.FloorToInt( hit.point.z/grid.TileSize));
            if (tileCoord != SelectedTile?.Position)
            {
                SelectedTile = grid.Nodes[tileCoord.x, tileCoord.y];
                //TODO: selection highlight object
                tileSelectionHighlighterTransform.position = new Vector3(SelectedTile.Position.x*grid.TileSize,0f,SelectedTile.Position.y*grid.TileSize);
                Debug.Log(hit.point);//TODO: Remove logs
            }
            
        }
        else
        {
            Debug.LogWarning("selected outside the terrain!");
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
            
            Instantiate(wallPrefab, tileSelectionHighlighterTransform.position, quaternion.identity);
            SelectedTile.StateNode = EnumStateNode.wall;
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
