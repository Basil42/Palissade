using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

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
    private Node selectedTile;
    [SerializeField] Level grid;
    private Vector3 mousePos;
    private Camera camRef;
    [SerializeField] private Transform tileSelectionHighlighterTransform;
    [SerializeField] private GameObject wallPrefab;//TODO some kind of 3D tileset, good luck future me
    private Queue<WallPiece> PieceQueue = new();//most likely only 2  pieces long at most
    private WallPiece heldPiece;//piece that the player is currently placing
    public int piecesPerRound = 15;
    [Header("UI")] 
    [SerializeField] Image _pieceDisplay;
    #region init and data
    private void Awake()
    {
        camRef = Camera.main;
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
    //Not cleaning up on destroy, don't tell anyone
    private void OnEnable()
    {
        PopulatePieceQueue();
        heldPiece = PieceQueue.Dequeue();
        SetPieceDisplay(heldPiece.type);
        _pieceDisplay.enabled = true;
    }

    

    

    private void OnDisable()
    {
        PieceQueue.Clear();
    }

    #endregion

    #region input
    private RaycastHit hit;
    private Vector2Int tileCoord;
    void Update()
    {
        RaycastHit hit;

        // Old Basile Version
        mousePos = Input.mousePosition;
        Ray mouseRay = camRef.ScreenPointToRay(mousePos);
        //selection
        if (Physics.Raycast(mouseRay, out hit))
        {
            
            tileCoord = new Vector2Int(Mathf.FloorToInt(hit.point.x/grid.TileSize),Mathf.FloorToInt( hit.point.z/grid.TileSize));
            if (tileCoord != selectedTile?.Position)
            {
                selectedTile = grid.Nodes[tileCoord.x, tileCoord.y];
                //TODO: selection highlight object

                tileSelectionHighlighterTransform.position = new Vector3(selectedTile.Position.x*grid.TileSize,0f,selectedTile.Position.y*grid.TileSize);

            }
            
        }
        else
        {
            Debug.LogWarning("selected outside the terrain!");
        }

        /* //Test Antoine version
        //Capte la position de la souris
        mousePos = Input.mousePosition;
        // Rayon de la souris par rapport  la camra
        Ray ray = camRef.ScreenPointToRay(mousePos);
        // Cration d'une variable Raycast ncessaire pour le out de la fonction Raycast

        //Si le raycast de la souris sur l'ecran est en contact avec un objet de layer "Structure"
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);

            selectedTile = GetNodeWithCoords(hit.collider.transform.position.x, hit.transform.position.z);

            //TODO: selection highlight object
            tileSelectionHighlighterTransform.position = new Vector3(selectedTile.Position.x * grid.TileSize, 0f, selectedTile.Position.y * grid.TileSize);
        }
        else
        {
            selectedTile = null;
            return;
        }*/

        //confirm
        if (Input.GetMouseButtonDown(0))
        {
            if (!CanConstructWallOn(selectedTile))
            {
                StopAllCoroutines();
                StartCoroutine(HighlighterErrorFeedback());
                return;
            }
            //TODO: apply the whole tetris piece after checking the relevant tiles for eligibility 
            
            Instantiate(wallPrefab, tileSelectionHighlighterTransform.position, quaternion.identity);
            selectedTile.StateNode = EnumStateNode.wall;
            var targetPosition = new Vector2(selectedTile.Coord.x, selectedTile.Coord.y) * grid.TileSize;
            var tileSize = grid.TileSize;
            var neutralHeight = 0f;//set it to max expected height if we reuse the ray-casting trick

            for (int i = 0; i < heldPiece.tiles.Length; i++)
            {
                var currentWorkingTile = selectedTile.Coord + heldPiece.tiles[i];
                Instantiate(standalone, new Vector3( currentWorkingTile.x * tileSize,neutralHeight,currentWorkingTile.y *tileSize), quaternion.identity);
                grid.Nodes[currentWorkingTile.x, currentWorkingTile.y].StateNode = EnumStateNode.wall;
            }

            heldPiece = PieceQueue.Dequeue();
            SetPieceDisplay(heldPiece.type);
            if (PieceQueue.Count == 0)
            {
                if(CheckRampartAreValid())
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
            RotatePiece90Clockwise(ref heldPiece);
        }
    }
    
    private bool CanConstructWallOn(Node selection)
    {
        for (int i = 0; i < heldPiece.tiles.Length; i++)
        {
            var pieceTile = heldPiece.tiles[i];
            var tile = grid.Nodes[selection.Coord.x + pieceTile.x, selection.Coord.y + pieceTile.y];
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
        for (int i = 0; i < piece.tiles.Length; i++)
        {
            var value = piece.tiles[i];
            piece.tiles[i] = new Vector2Int(value.y, -value.x);
        }
    }
    #endregion
    
    

    private WallPiece GetRandomPiece()
    {
        var typeSelect = Random.Range(0, 3);
        WallPiece piece = new WallPiece((WallpieceType)typeSelect);
        
        //TODO: add random rotation
        return piece;
    }
    private void PopulatePieceQueue()
    {
        for (int i = 0; i < piecesPerRound; i++)//I'd normally do just a few at a time but this is less code
        {
            PieceQueue.Enqueue(GetRandomPiece());
        }
    }

    [SerializeField] private Sprite pointSprite;
    [SerializeField] private Sprite cornerSprite;
    [SerializeField] private Sprite barSprite;
    private void SetPieceDisplay(WallpieceType heldPieceType)
    {
        switch (heldPieceType)
        {
            case WallpieceType.Point:
                _pieceDisplay.sprite = pointSprite;
                break;
            case WallpieceType.Corner:
                _pieceDisplay.sprite = cornerSprite;
                break;
            case WallpieceType.Bar:
                _pieceDisplay.sprite = barSprite;
                break;
            default:
                _pieceDisplay.sprite = null;
                break;
        }

        _pieceDisplay.rectTransform.rotation = quaternion.identity;
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
    //TODO: finish tileset rules
    private void RunTileSetRules(WallPiece piece, Vector2Int origin)//gaze not unto the abyss lest it gazes back unto you
    {
        for (int i = 0; i < piece.tiles.Length; i++)
        {
            var pieceTile = piece.tiles[i];
            var targetCoordinates = new Vector2(origin.x + pieceTile.x, origin.y + pieceTile.y) * grid.TileSize;
            var flag = NeighboringWallsFlagCheck(origin);
            //Cross case
            if (flag == (NeighboringWalls.Down | NeighboringWalls.Up | NeighboringWalls.Left | NeighboringWalls.Right))
            {

                Instantiate(crossWall, new Vector3(targetCoordinates.x, 0f, targetCoordinates.y),
                    quaternion.identity);
                
            }
            //corner path
            
        }
        
    }

    private void RunTileSetRules(Node tile, Vector2Int coordinates)
    {
        
    }

    private NeighboringWalls NeighboringWallsFlagCheck(Vector2Int origin)
    {
        NeighboringWalls flag = ((grid.Nodes[origin.x - 1, origin.y].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Left
                                    : NeighboringWalls.None) |
                                ((grid.Nodes[origin.x - 1, origin.y + 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.UpLeft
                                    : NeighboringWalls.None) |
                                ((grid.Nodes[origin.x, origin.y + 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Up
                                    : NeighboringWalls.None) |
                                ((grid.Nodes[origin.x + 1, origin.y + 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.UpRight
                                    : NeighboringWalls.None) |
                                ((grid.Nodes[origin.x + 1, origin.y].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Right
                                    : NeighboringWalls.None) |
                                ((grid.Nodes[origin.x + 1, origin.y - 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.DownRight
                                    : NeighboringWalls.None) |
                                ((grid.Nodes[origin.x, origin.y - 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.Down
                                    : NeighboringWalls.None) |
                                ((grid.Nodes[origin.x - 1, origin.y - 1].StateNode == EnumStateNode.wall)
                                    ? NeighboringWalls.DownLeft
                                    : NeighboringWalls.None);
        return flag;
    }

    #endregion
    
    /////////////////////////////////////////// TEST VALID ZONE PATHFINDING ///////////////////////////////////
    #region

    private Node currentNode;
    public Queue<Node> frontier;
    private List<Node> came_from;
    private Dictionary<Node, Node> wayDico;
    private bool wayIsValid;

    public bool CheckRampartAreValid()
    {
        wayIsValid = true;
        frontier = new Queue<Node>();
        came_from = new List<Node>();
        wayDico = new Dictionary<Node, Node>();

        // 1,1 c'est la position free garantie
        grid[1, 1].StateNode = EnumStateNode.castle;
        frontier.Enqueue(grid[1,1]);
        came_from.Add(grid[1, 1]);


        while (frontier.Count != 0)
        {
            // On prend le premier node dans la Queue et on teste ses voisins
            currentNode = frontier.Dequeue();
            TestNeighbors(currentNode);
            came_from.Add(currentNode);

            // Et arr�ter si on a crois� un node water
            if (!wayIsValid)
            {
                Debug.Log("Remparts mauvais");
                return false;
            }
        }

        Debug.Log("Toutes les tiles ont �t�s test�es et valides");
        return true;
    }

    private void TestNeighbors(Node node)
    {
        // On teste les 4 voisins du node si ils existent
        if (node.Coord.y + 1 < grid.Height)
            TestNeighbor(grid.Nodes[node.Coord.x, node.Coord.y + 1]);
        if (node.Coord.y - 1 >= 0)
            TestNeighbor(grid.Nodes[node.Coord.x, node.Coord.y - 1]);
        if (node.Coord.x + 1 < grid.Width)
            TestNeighbor(grid.Nodes[node.Coord.x + 1, node.Coord.y]);
        if (node.Coord.x - 1 >= 0)
            TestNeighbor(grid.Nodes[node.Coord.x - 1, node.Coord.y]);
    }

    private void TestNeighbor(Node neighbor)
    {
        // Si diff�rent de l� o� on vient
        for (int i = 0; i < came_from.Count; i++)
        {
            if (neighbor == came_from[i])
                return;
        }
        // Et libre
        if (neighbor.StateNode == EnumStateNode.buildable)
        {
            // On ajoute le node � la liste des trucs � g�rer
            // Et on met dans le dico
            if (!wayDico.ContainsKey(neighbor))
            {
                frontier.Enqueue(neighbor);
                wayDico.Add(neighbor, currentNode);
            }

        }
        else if(neighbor.StateNode == EnumStateNode.water)
        {
            wayIsValid = false;
        }
    }

    #endregion
}
