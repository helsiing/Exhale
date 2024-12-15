using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create TilesCollection", fileName = "TilesCollection", order = 0)]
    public class TilesTemplateCollection : ScriptableObjectCollection<TileTemplate>
    {
        public GameObject GetRandom()
        {
            return Values[Random.Range(0, Values.Count)].BoardPrefab;
        }
    }
}
