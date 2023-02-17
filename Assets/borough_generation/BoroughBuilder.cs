using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace borough_generation
{
    public class BoroughBuilder : MonoBehaviour
    {
        [SerializeReference] private Terrain terrainRef;
        [SerializeField] private List<BoroughBehavior> boroughPrefabs;
        
        [SerializeField] private float maxTerrainHeight = 10.0f;

        [Header("Debug")] [SerializeField] private bool debug;

        private void Start()
        {
            if (debug)
            {
                if(LevelManager.Instance.LevelRef.Nodes[0, 0] != null)CreateBorough(LevelManager.Instance.LevelRef.Nodes[0,0]);
            }
        }

        private void ExtendBoroughs()
        {
            foreach (var tile in LevelManager.Instance.LevelRef.Nodes)
            {
                if (tile.ShouldCreateBorough())
                {
                    CreateBorough(tile);
                }
            }
        }

        private void CreateBorough(Node tile)
        {
            var randomPrefabIndex = Random.Range(0, boroughPrefabs.Count-1);
            var tileCoord = tile.Position;
            BoroughBehavior newBorough = Instantiate(boroughPrefabs[randomPrefabIndex],
                new Vector3(tileCoord.x, maxTerrainHeight, tileCoord.y), 
                Quaternion.identity);
            newBorough.Tile = tile;
        }
    }

    public static class BoroughUtility
    {
        public static bool ShouldCreateBorough(this Node tile)
        {
            //is the tile already a borough, a twoer or water?
            if (tile.StateNode == EnumStateNode.building || tile.StateNode == EnumStateNode.tower || tile.StateNode == EnumStateNode.water) return false;
            //is the tile under a player/opponent control
            if (tile.NodeController == EnumNodeControl.playerControlled ||
                tile.NodeController == EnumNodeControl.ennemyControlled) return true;
            return false;//default case, a tile not controlled by anyone.
            //TODO: account for city growth
        }
    }
}
