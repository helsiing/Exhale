using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    public enum TileType
    {
        Empty = 0,
        Ground = 1,
        Building = 2
    }
   
    public partial class TileTemplate : ScriptableObjectCollectionItem
    {
        [SerializeField]
        private TileType type;
        
        [SerializeField]
        private GameObject boardPrefab;
        public GameObject BoardPrefab => boardPrefab;
    }
}
