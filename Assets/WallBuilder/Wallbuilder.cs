using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Wallbuilder : MonoBehaviour
{
    private Node selectedTile;
    [SerializeField] Level grid;
    private Vector3 mousePos;
    private Camera camRef;
    [SerializeField] private Transform tileSelectionHighlighterTransform;
    [SerializeField] private GameObject wallPrefab;//TODO some kind of 3D tileset, good luck future me

    private Node castle; // Node to begin for valid rempart testing

    private void Awake()
    {
        camRef = Camera.main;
    }

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
                //Debug.Log(hit.point);//TODO: Remove logs
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
            Debug.Log(selectedTile.Coord + ",  " + selectedTile.StateNode);
            //TODO: check all tile covered by the piece
            if (CannotConstructWallOn(selectedTile))
            {
                StopAllCoroutines();
                StartCoroutine(HighlighterErrorFeedback());
                return;
            }
            //TODO: apply the whole tetris piece after checking the relevant tiles for eligibility 
            
            Instantiate(wallPrefab, tileSelectionHighlighterTransform.position, quaternion.identity);
            selectedTile.StateNode = EnumStateNode.wall;
        }
    }

    public Node GetNodeWithCoords(float x, float z)
    {
        return grid[(int)x, (int)z];
    }
   
    private bool CannotConstructWallOn(Node tile)
    {
        return tile.StateNode == EnumStateNode.water ||
               tile.StateNode == EnumStateNode.tower ||
               tile.StateNode == EnumStateNode.wall ||
               tile.StateNode == EnumStateNode.castle ||
               tile == null;
    }
    
    private IEnumerator HighlighterErrorFeedback()
    {
        //TODO: make the highlighter flash red
        yield break;
    }

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

            // Et arrêter si on a croisé un node water
            if (!wayIsValid)
            {
                Debug.Log("Remparts mauvais");
                return false;
            }
        }

        Debug.Log("Toutes les tiles ont étés testées et valides");
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
        // Si différent de là où on vient
        for (int i = 0; i < came_from.Count; i++)
        {
            if (neighbor == came_from[i])
                return;
        }
        // Et libre
        if (neighbor.StateNode == EnumStateNode.buildable)
        {
            // On ajoute le node à la liste des trucs à gérer
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
