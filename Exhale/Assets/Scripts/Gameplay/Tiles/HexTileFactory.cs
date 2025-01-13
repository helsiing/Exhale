using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public static class HexTileFactory
    {
        public static GameObject GetRandomTile(bool shouldInstantiate)
        {
            List<HexTileTemplate> tiles = HexTilesTemplateCollection.Values.ToList();
            int count = tiles.Count;
            return shouldInstantiate ? Object.Instantiate(tiles[Random.Range(0, count)].BoardPrefab) : tiles[Random.Range(0, count)].BoardPrefab;
        }
    }
}