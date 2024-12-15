using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exhale.Scripts.Data
{
    public partial class CellGroundTemplate : ScriptableObjectCollectionItem
    {
        [SerializeField] private GameObject boardPrefab;
        public GameObject BoardPrefab => boardPrefab;
    }
}
