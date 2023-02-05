using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Level levelRef;

    void Awake()
    {
        levelRef.initializeLevel();
    }

    // Gizmos qui affiche la map

    private void OnDrawGizmos()
    {
        for (int y = 0; y < levelRef.Height; y++)
        {
            for (int x = 0; x < levelRef.Width; x++)
            {
                switch (levelRef[x, y].StateNode)
                {
                    case EnumStateNode.water:
                        Gizmos.color = Color.blue;
                        break;
                    case EnumStateNode.buildable:
                        Gizmos.color = Color.green;
                        break;
                    case EnumStateNode.wall:
                        Gizmos.color = Color.grey;
                        break;
                    default:
                        Gizmos.color = Color.white;
                        break;
                }

                Gizmos.DrawCube(new Vector3(x, y) + transform.position, Vector2.one);
            }
        }
    }
}
