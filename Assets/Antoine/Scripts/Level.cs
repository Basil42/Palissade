using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    //TODO: store pregenerated tiles in the SO
    private Node[,] _nodes = new Node[0,0]; public Node[,] Nodes => _nodes;

    [SerializeField] int _width = 3; public int Width => _width;

    [SerializeField] int _height = 3; public int Height => _height;

    [SerializeField] private float _tileSize = 10f;
    public float TileSize => _tileSize;

    public Node this[int x, int y] => _nodes[x,y];

    public Level(int width, int height)
    {
        _width = width;
        _height = height;

        initializeLevel();
    }

    internal void initializeLevel()
    {
        _nodes = new Node[_width, _height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _nodes[x, y] = new Node(x, y);

                // Chateau
                if (x == 20 && y == 20) _nodes[x, y].StateNode = EnumStateNode.castle;
                if (x == 21 && y == 20) _nodes[x, y].StateNode = EnumStateNode.castle;
                if (x == 20 && y == 21) _nodes[x, y].StateNode = EnumStateNode.castle;
                if (x == 21 && y == 21) _nodes[x, y].StateNode = EnumStateNode.castle;

                // ligne mur gauche
                for (int i = 0; i < 8; i++)
                {
                    if (x == 17 && y == 17 + i ) _nodes[x, y].StateNode = EnumStateNode.wall;
                }

                //Ligne mur droit
                for (int i = 0; i < 8; i++)
                {
                    if (x == 24 && y == 17 + i) _nodes[x, y].StateNode = EnumStateNode.wall;
                }

                //Ligne mur bas
                for (int i = 0; i < 8; i++)
                {
                    if (x == 17 + i && y == 17) _nodes[x, y].StateNode = EnumStateNode.wall;
                }

                //Ligne mur haut
                for (int i = 0; i < 8; i++)
                {
                    if (x == 17 + i && y == 24) _nodes[x, y].StateNode = EnumStateNode.wall;
                }

                // Test state
                //if (x == 2 && y == 2) _nodes[x, y].StateNode = EnumStateNode.wall;
                //if (x == 5 && y == 5) _nodes[x, y].StateNode = EnumStateNode.water;
                //if (x == 5 && y == 4) _nodes[x, y].StateNode = EnumStateNode.water;
            }
        }
    }
}