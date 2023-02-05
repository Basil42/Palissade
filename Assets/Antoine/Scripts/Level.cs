using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    private Node[,] _nodes = null; public Node[,] Nodes => _nodes;

    [SerializeField] int _width = 3; public int Width => _width;

    [SerializeField] int _height = 3; public int Height => _height;

    [SerializeField] private float _tileSize = 10f;
    public float TileSize => _tileSize;

    public Node this[int x, int y] => _nodes[x,y];

    public Level(int widht, int height)
    {
        _width = widht;
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

                // Test state
                if (x == 2 && y == 2) _nodes[x, y].StateNode = EnumStateNode.wall;
                if (x == 5 && y == 5) _nodes[x, y].StateNode = EnumStateNode.water;
                if (x == 5 && y == 4) _nodes[x, y].StateNode = EnumStateNode.water;
            }
        }
    }
}