using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WallpieceType
{
    Point,
    Corner,
    Bar
}
public struct WallPiece
{
    public Vector2Int[] tiles;
    public WallpieceType type;

    public WallPiece(WallpieceType type)
    {
        switch (type)
        {
            case WallpieceType.Point:
                this.tiles = new Vector2Int[1];
                this.tiles[0] = Vector2Int.zero;
                this.type = WallpieceType.Point;
                break;
            case WallpieceType.Corner:
                this.tiles = new Vector2Int[3];
                this.tiles[0] = Vector2Int.left;
                this.tiles[1] = Vector2Int.zero;
                this.tiles[2] = Vector2Int.up;
                this.type = WallpieceType.Corner;
                break;
            case WallpieceType.Bar:
                this.tiles = new Vector2Int[3];
                this.tiles[0] = Vector2Int.left;
                this.tiles[1] = Vector2Int.zero;
                this.tiles[2] = Vector2Int.right;
                this.type = WallpieceType.Bar;
                break;
            default:
                Debug.LogWarning("invalid tile type, generating point tile");
                this.tiles = new Vector2Int[1];
                this.tiles[0] = Vector2Int.zero;
                this.type = WallpieceType.Point;
                break;
        }
        
    }
}


public class Wallbuilder : MonoBehaviour//Only enable while placing walls
{
    private Node SelectedTile;
    [SerializeField] Level grid;
    private Vector3 mousePos;
    private Camera camRef;
    [SerializeField] private Transform tileSelectionHighlighterTransform;
    [SerializeField] private GameObject wallPrefab;//TODO some kind of 3D tileset, good luck future me
    private Queue<WallPiece> PieceQueue;//most likely only 2  pieces long at most
    
    private void Awake()
    {
        camRef = Camera.main;
    }

    private void OnEnable()
    {
        PopulatePieceQueue();
    }

    private void PopulatePieceQueue()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        PieceQueue.Clear();
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

    private WallPiece GetRandomPiece()
    {
        var typeSelect = Random.Range(0, 2);
        WallPiece piece = new WallPiece((WallpieceType)typeSelect);
        
        //TODO: add random rotation
        return piece;
    }

    internal void RotatePiece90Clockwise(ref WallPiece piece)//rotation around the 0,0 axis
    {
        for (int i = 0; i < piece.tiles.Length; i++)
        {
            var value = piece.tiles[i];
            piece.tiles[i] = new Vector2Int(value.y, -value.x);
        }
    }
}
