using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exhale.Scripts.Data
{
    public partial class CellGroundTemplate : ScriptableObjectCollectionItem
    {
        [FormerlySerializedAs("elementType")] [FormerlySerializedAs("type")] [SerializeField]
        private CellElement element;
        public CellElement Element => element;
        
        [SerializeField] private GameObject boardPrefab;
        public GameObject BoardPrefab => boardPrefab;
    }
}
