using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Level levelRef; public Level LevelRef => levelRef;
    public GameObject castle;//this is a prefab
    [Header("setup")] [SerializeField] private List<Vector2Int> castlesPositions;
    void Awake()
    {
        levelRef.initializeLevel();

        foreach (Vector2Int position in castlesPositions)
        {
            Instantiate(castle, levelRef.GetCenterWorldPosition(position),Quaternion.identity);
            levelRef.Nodes[position.x, position.y].StateNode = EnumStateNode.castle;
        }
    }

    // Gizmos qui affiche la map

    // private void OnDrawGizmos()
    // {
    //     if (levelRef.Nodes == null) return;//not sure why Nodes is null at editor time
    //     for (int y = 0; y < levelRef.Nodes.GetLength(0); y++)
    //     {
    //         for (int x = 0; x < LevelRef.Nodes.GetLength(1); x++)
    //         {
    //             switch (levelRef[x, y].StateNode)
    //             {
    //                 case EnumStateNode.water:
    //                     Gizmos.color = Color.blue;
    //                     break;
    //                 case EnumStateNode.buildable:
    //                     Gizmos.color = Color.green;
    //                     break;
    //                 case EnumStateNode.wall:
    //                     Gizmos.color = Color.grey;
    //                     break;
    //                 case EnumStateNode.tower:
    //                     Gizmos.color = Color.black;
    //                     break;
    //                 case EnumStateNode.castle:
    //                     Gizmos.color = Color.red;
    //                     break;
    //                 default:
    //                     Gizmos.color = Color.white;
    //                     break;
    //             }
    //
    //             Gizmos.DrawCube(new Vector3(x, y) + transform.position, Vector2.one);
    //         }
    //     }
    // }

    internal void Construct(Node target, GameObject prefab)
    {
        var targetPosition =
            new Vector3(target.Position.x * levelRef.TileSize, 0f, target.Position.y * levelRef.TileSize);
        var constructedObject = Instantiate(prefab, targetPosition, Quaternion.identity);
        target.SetConstruct(constructedObject);
    }
}
