using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public enum WallPieceType
{
    Point,
    Corner,
    Bar
}
public readonly struct WallPiece
{
    public readonly Vector2Int[] Tiles;
    public readonly WallPieceType Type;

    public void Rotate90Degrees()
    {
        for (int i = 0; i < Tiles.Length; i++)
        {
            var value = Tiles[i];
            Tiles[i] = new Vector2Int(value.y, -value.x);
        }
    }
    public WallPiece(WallPieceType type)
    {
        switch (type)
        {
            case WallPieceType.Point:
                this.Tiles = new Vector2Int[1];
                this.Tiles[0] = Vector2Int.zero;
                this.Type = WallPieceType.Point;
                break;
            case WallPieceType.Corner:
                this.Tiles = new Vector2Int[3];
                this.Tiles[0] = Vector2Int.left;
                this.Tiles[1] = Vector2Int.zero;
                this.Tiles[2] = Vector2Int.up;
                this.Type = WallPieceType.Corner;
                break;
            case WallPieceType.Bar:
                this.Tiles = new Vector2Int[3];
                this.Tiles[0] = Vector2Int.left;
                this.Tiles[1] = Vector2Int.zero;
                this.Tiles[2] = Vector2Int.right;
                this.Type = WallPieceType.Bar;
                break;
            default:
                Debug.LogWarning("invalid tile type, generating point tile");
                this.Tiles = new Vector2Int[1];
                this.Tiles[0] = Vector2Int.zero;
                this.Type = WallPieceType.Point;
                break;
        }
        
    }
}


public class Wallbuilder : MonoBehaviour//Only enable while placing walls
{
    Level _grid;
    [SerializeField] private Transform tileSelectionHighlighterTransform;
    [SerializeField] private GameObject wallPrefab;//TODO some kind of 3D tileset, good luck future me
    private readonly Queue<WallPiece> _pieceQueue = new();//most likely only 2  pieces long at most
    private WallPiece _heldPiece;//piece that the player is currently placing
    public int piecesPerRound = 15;
    [FormerlySerializedAs("_pieceDisplay")]
    [Header("UI")] 
    [SerializeField] Image pieceDisplay;
    #region init and data
    private void Awake()
    {
        GameManager.OnEnterWallMode += () =>
        {
            this.enabled = true;
            Debug.Log("Wall mode");
        };
        GameManager.OnExitWallMode += () =>
        {
            this.enabled = false;
            Debug.Log("Wall mode stop");
        };
    }

    private void Start()
    {
        if (GameManager.Instance.ActualGameMode != GameMode.RampartMode) enabled = false;
    }

    //Not cleaning up on destroy, don't tell anyone
    private void OnEnable()
    {
        _grid = LevelManager.Instance.LevelRef;
        PopulatePieceQueue();
        _heldPiece = _pieceQueue.Dequeue();
        SetPieceDisplay(_heldPiece.Type);
        pieceDisplay.enabled = true;
    }

    

    

    private void OnDisable()
    {
        _pieceQueue.Clear();
    }

    #endregion

    #region input
    private RaycastHit _hit;
    private Vector2Int _tileCoord;
    void Update()
    {
        var _selectedTile = TileSelector.SelectedTile;
        if (_tileCoord != TileSelector.SelectedTile?.Position && _selectedTile != null)
        {
            _tileCoord = _selectedTile.Position;
            tileSelectionHighlighterTransform.position = new Vector3( _selectedTile.Position.x*_grid.TileSize,0f,_selectedTile.Position.y*_grid.TileSize);

        }
        //confirm
        if (Input.GetMouseButtonDown(0))
        {
            if (!CanConstructWallOn(_selectedTile))
            {
                StopAllCoroutines();
                StartCoroutine(HighlighterErrorFeedback());
                return;
            }
            //TODO: apply the whole tetris piece after checking the relevant tiles for eligibility 
            
            Instantiate(wallPrefab, tileSelectionHighlighterTransform.position, quaternion.identity);
            _selectedTile.StateNode = EnumStateNode.wall;
            var tileSize = _grid.TileSize;
            var neutralHeight = 0f;//set it to max expected height if we reuse the ray-casting trick

            for (int i = 0; i < _heldPiece.Tiles.Length; i++)
            {
                var currentWorkingTile = _selectedTile.Coord + _heldPiece.Tiles[i];
                Instantiate(standalone, new Vector3( currentWorkingTile.x * tileSize,neutralHeight,currentWorkingTile.y *tileSize), quaternion.identity);
                _grid.Nodes[currentWorkingTile.x, currentWorkingTile.y].StateNode = EnumStateNode.wall;
            }

            _heldPiece = _pieceQueue.Dequeue();
            SetPieceDisplay(_heldPiece.Type);
            if (_pieceQueue.Count == 0)
            {
                if(ZoneOfControl.Instance.CheckRampartAreValid())
                {
                    GameManager.Instance.NextMode();
                }
                else
                {
                    Debug.Log("GAME OVER");
                    SceneManager.LoadScene(0);
                }
            }
        }
        //rotate piece
        if (Input.GetMouseButtonDown(1))
        {
            RotatePiece90Clockwise(ref _heldPiece);
        }
    }
    
    private bool CanConstructWallOn(Node selection)
    {
        for (int i = 0; i < _heldPiece.Tiles.Length; i++)
        {
            var pieceTile = _heldPiece.Tiles[i];
            var tile = _grid.Nodes[selection.Coord.x + pieceTile.x, selection.Coord.y + pieceTile.y];
            if (tile.StateNode == EnumStateNode.water ||
                tile.StateNode == EnumStateNode.tower ||
                tile.StateNode == EnumStateNode.wall ||
                tile.StateNode == EnumStateNode.castle) return false;
        }

        return true;
    }
    
    private IEnumerator HighlighterErrorFeedback()
    {
        //TODO: make the highlighter flash red
        yield break;
    }

    internal void RotatePiece90Clockwise(ref WallPiece piece)//rotation around the 0,0 axis
    {
        for (int i = 0; i < piece.Tiles.Length; i++)
        {
            var value = piece.Tiles[i];
            piece.Tiles[i] = new Vector2Int(value.y, -value.x);
        }
    }
    #endregion
    
    

    private WallPiece GetRandomPiece()
    {
        var typeSelect = Random.Range(0, 3);
        WallPiece piece = new WallPiece((WallPieceType)typeSelect);
        
        //TODO: add random rotation
        return piece;
    }
    private void PopulatePieceQueue()
    {
        for (int i = 0; i < piecesPerRound; i++)//I'd normally do just a few at a time but this is less code
        {
            _pieceQueue.Enqueue(GetRandomPiece());
        }
    }

    [SerializeField] private Sprite pointSprite;
    [SerializeField] private Sprite cornerSprite;
    [SerializeField] private Sprite barSprite;
    private void SetPieceDisplay(WallPieceType heldPieceType)
    {
        switch (heldPieceType)
        {
            case WallPieceType.Point:
                pieceDisplay.sprite = pointSprite;
                break;
            case WallPieceType.Corner:
                pieceDisplay.sprite = cornerSprite;
                break;
            case WallPieceType.Bar:
                pieceDisplay.sprite = barSprite;
                break;
            default:
                pieceDisplay.sprite = null;
                break;
        }

        pieceDisplay.rectTransform.rotation = quaternion.identity;
    }
    #region Tilesets

    [Flags]
    enum NeighboringWalls
    {
        None = 0,
        Left = 1,
        UpLeft = 2,
        Up = 4,
        UpRight = 8,
        Right = 16,
        DownRight = 32,
        Down = 64,
        DownLeft = 128
    }
    //TODO: replace by SO containing references to a sprite and a mesh
    [Header("wall prefabs")] 
    [SerializeField] private GameObject crossWall;
    [SerializeField] private GameObject cornerPath;
    [SerializeField] private GameObject deadEnd;
    [SerializeField] private GameObject tIntersection;
    [SerializeField] private GameObject walkPath;
    [SerializeField] private GameObject invertedCorner;//fully surrounded exept a diagonal tile
    [SerializeField] private GameObject flat;
    [SerializeField] private GameObject corner;//three consecutive neighbors
    [SerializeField] private GameObject standalone;
    private void RunTileSetRulesOnHeldPiece()//updating the highlighter to show the proper piece
    {
        
    }
    //TODO: finish tile set rules
    private void RunTileSetRules(WallPiece piece, Vector2Int origin)//gaze not unto the abyss lest it gazes back unto you
    {
        for (int i = 0; i < piece.Tiles.Length; i++)
        {
            var pieceTile = piece.Tiles[i];
            var targetCoordinates = new Vector2(origin.x + pieceTile.x, origin.y + pieceTile.y) * _grid.TileSize;
            var flag = NeighboringWallsFlagCheck(origin);
            //Cross case, easy as rotation doesn't matter
            if (flag == (NeighboringWalls.Down | NeighboringWalls.Up | NeighboringWalls.Left | NeighboringWalls.Right))
            {

                Instantiate(crossWall, new Vector3(targetCoordinates.x, 0f, targetCoordinates.y),
                    quaternion.identity);
                
            }
            //corner path
            //TODO: finish tileset rules
        }
        
    }

    private void RunTileSetRules(Node tile, Vector2Int coordinates)
    {
        
    }

    private NeighboringWalls NeighboringWallsFlagCheck(Vector2Int origin)
    {
        NeighboringWalls flag = ((_grid.Nodes[origin.x - 1, origin.y].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Left
                                    : NeighboringWalls.None) |
                                ((_grid.Nodes[origin.x - 1, origin.y + 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.UpLeft
                                    : NeighboringWalls.None) |
                                ((_grid.Nodes[origin.x, origin.y + 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Up
                                    : NeighboringWalls.None) |
                                ((_grid.Nodes[origin.x + 1, origin.y + 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.UpRight
                                    : NeighboringWalls.None) |
                                ((_grid.Nodes[origin.x + 1, origin.y].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Right
                                    : NeighboringWalls.None) |
                                ((_grid.Nodes[origin.x + 1, origin.y - 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.DownRight
                                    : NeighboringWalls.None) |
                                ((_grid.Nodes[origin.x, origin.y - 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Down
                                    : NeighboringWalls.None) |
                                ((_grid.Nodes[origin.x - 1, origin.y - 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.DownLeft
                                    : NeighboringWalls.None);
        return flag;
    }

    #endregion
    
}
