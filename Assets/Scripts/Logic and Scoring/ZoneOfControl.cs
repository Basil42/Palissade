using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneOfControl : Singleton<ZoneOfControl>
{
    #region
    
    private Node currentNode;
    public Queue<Node> frontier;
    private List<Node> came_from;
    private Dictionary<Node, Node> wayDico;
    private bool wayIsValid;
    private Level _grid;
    private void Start()
    {
        _grid = LevelManager.Instance.LevelRef;
    }

    public bool CheckRampartAreValid()
    {
        wayIsValid = true;
        frontier = new Queue<Node>();
        came_from = new List<Node>();
        wayDico = new Dictionary<Node, Node>();

        // 22,22 c'est la position devant le chateau
        _grid[22, 22].StateNode = EnumStateNode.castle;
        frontier.Enqueue(_grid[1,1]);
        came_from.Add(_grid[1, 1]);


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
