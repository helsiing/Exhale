using System.Linq;
using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public static class TileFactory
    {
        public static GameObject GetRandomTile(bool shouldInstantiate)
        {
            var tiles = TilesTemplateCollection.Values.ToList();
            int count = tiles.Count;
            return shouldInstantiate ? Object.Instantiate(tiles[Random.Range(0, count)].BoardPrefab) : tiles[Random.Range(0, count)].BoardPrefab;
        }
    }
}