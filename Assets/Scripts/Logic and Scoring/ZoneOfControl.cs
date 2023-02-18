using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZoneOfControl : Singleton<ZoneOfControl>
{
    #region
    
    private Node _currentNode;
    public Queue<Node> Frontier;
    private List<Node> _cameFrom;
    private Dictionary<Node, Node> _wayDico;
    private bool _wayIsValid;
    private Level _grid;

    

    private void Start()
    {
        _grid = LevelManager.Instance.LevelRef;
    }

    public bool CheckRampartAreValid(Node castleNode)
    {
        _wayIsValid = true;
        Frontier = new Queue<Node>();
        _cameFrom = new List<Node>();
        _wayDico = new Dictionary<Node, Node>();

        // 22,22 c'est la position devant le chateau
        _grid[22, 22].StateNode = EnumStateNode.castle;
        Frontier.Enqueue(_grid[1,1]);
        _cameFrom.Add(_grid[1, 1]);


        while (Frontier.Count != 0)
        {
            // On prend le premier node dans la Queue et on teste ses voisins
            _currentNode = Frontier.Dequeue();
            TestNeighbors(_currentNode);
            _cameFrom.Add(_currentNode);

            // Et arr�ter si on a crois� un node water
            if (!_wayIsValid)
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
        if (node.Coord.y + 1 < _grid.Height)
            TestNeighbor(_grid.Nodes[node.Coord.x, node.Coord.y + 1]);
        if (node.Coord.y - 1 >= 0)
            TestNeighbor(_grid.Nodes[node.Coord.x, node.Coord.y - 1]);
        if (node.Coord.x + 1 < _grid.Width)
            TestNeighbor(_grid.Nodes[node.Coord.x + 1, node.Coord.y]);
        if (node.Coord.x - 1 >= 0)
            TestNeighbor(_grid.Nodes[node.Coord.x - 1, node.Coord.y]);
    }

    private void TestNeighbor(Node neighbor)
    {
        // Si diff�rent de l� o� on vient
        for (int i = 0; i < _cameFrom.Count; i++)
        {
            if (neighbor == _cameFrom[i])
                return;
        }
        // Et libre
        if (neighbor.StateNode == EnumStateNode.buildable)
        {
            // On ajoute le node � la liste des trucs � g�rer
            // Et on met dans le dico
            if (!_wayDico.ContainsKey(neighbor))
            {
                Frontier.Enqueue(neighbor);
                _wayDico.Add(neighbor, _currentNode);
            }

        }
        else if(neighbor.StateNode == EnumStateNode.water)
        {
            _wayIsValid = false;
        }
    }

    

    [Header("Animation")] [SerializeField] private float zoneOfControlAnimationStep = 0.3f;
    [SerializeField] private BoroughCollection boroughPrefabs; 
    internal IEnumerator ZoneOfControlAnimationRoutine()
    {
        var waiter = new WaitForSeconds(zoneOfControlAnimationStep);
        foreach (var node in _wayDico)
        {
            node.Key.NodeController = EnumNodeControl.playerControlled;//not great to do it here, but gotta go fast
            if (node.Key.StateNode == EnumStateNode.buildable)
            {
                var randomPrefabIndex = Random.Range(0, boroughPrefabs.boroughPrefabs.Count);
                LevelManager.Instance.Construct(node.Key,boroughPrefabs.boroughPrefabs[randomPrefabIndex]);
            }

            yield return waiter;
        }
    }
    #endregion
}
