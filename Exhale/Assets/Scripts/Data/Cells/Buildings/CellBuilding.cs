using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    public partial class CellBuilding : ScriptableObjectCollectionItem
    {
        [SerializeField]
        private GameObject boardPrefab;
        public GameObject BoardPrefab => boardPrefab;
    }
}
