using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create HexTilesCollection", fileName = "HexTilesCollection", order = 0)]
    public class HexTilesTemplateCollection : ScriptableObjectCollection<HexTileTemplate>
    {
        public GameObject GetRandom()
        {
            return Values[Random.Range(0, Values.Count)].BoardPrefab;
        }
    }
}
