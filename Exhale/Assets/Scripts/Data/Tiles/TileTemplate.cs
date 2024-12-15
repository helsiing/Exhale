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
        public TileType Type => type;
        
        [SerializeField]
        private int maxLevel;
        public int MaxLevel => maxLevel;
        
        [SerializeField]
        private GameObject boardPrefab;
        public GameObject BoardPrefab => boardPrefab;
    }
}
