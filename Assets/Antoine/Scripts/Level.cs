using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    private Node[,] _nodes = new Node[0,0]; public Node[,] Nodes => _nodes;
    [SerializeField] int _width = 3; public int Width => _width;
    [SerializeField] int _height = 3; public int Height => _height;
    [SerializeField] private float _tileSize = 10f;
    [SerializeField] private TextAsset tileTypeJson;
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
        EnumStateNode[][] tileTypes = JsonConvert.DeserializeObject<EnumStateNode[][]>(tileTypeJson.text);
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _nodes[x, y] = new Node(x, y) { StateNode = tileTypes[x][y]};
            }
        }

        
    }
    public Vector3 GetOffsetWorldPosition(Vector2Int coord)
    {
        return new Vector3((coord.x * _tileSize), 0f, (coord.y * _tileSize));
    }

    public Vector3 GetOffsetWorldPosition(int xCoord, int yCoord)
    {
        return GetOffsetWorldPosition(new Vector2Int(xCoord, yCoord));
    }
    public Vector3 GetCenterWorldPosition(Vector2Int coord)
    {
        return new Vector3((coord.x * _tileSize) + _tileSize/2.0f, 0f, (coord.y * _tileSize) + _tileSize/2.0f);
    }

    public Vector3 GetCenterWorldPosition(int xCoord, int yCoord)
    {
        return GetCenterWorldPosition(new Vector2Int(xCoord, yCoord));
    }
}