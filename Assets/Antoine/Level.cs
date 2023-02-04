using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class Level : ScriptableObject
{
    private Node[,] _nodes = null; public Node[,] Nodes => _nodes;

    [SerializeField] int _width = 3; public int Width => _width;

    [SerializeField] int _height = 3; public int Height => _height;

    public Node this[int x, int y] => _nodes[x,y];

    public Level(int widht, int height)
    {
        _width = widht;
        _height = height;

        _nodes = new Node[_width , _height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _nodes[x, y] = new Node(x, y);

                // Test state
                if (x == 1 && y == 1) _nodes[x, y].StateNode = EnumStateNode.water;
                if (x == 1 && y == 2) _nodes[x, y].StateNode = EnumStateNode.water;
            }
        }
    }
}