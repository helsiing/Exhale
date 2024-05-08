using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    public partial class HexCellGroundTemplate : ScriptableObjectCollectionItem
    {
        [SerializeField]
        private HexCellType type;
        public HexCellType Type => type;
        
        [SerializeField] private GameObject boardPrefab;
        public GameObject BoardPrefab => boardPrefab;
    }
}
