using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create CellBuildingsCollection", fileName = "CellBuildingsCollection", order = 0)]
    public class CellBuildingsTemplateCollection : ScriptableObjectCollection<CellBuildingTemplate>
    {
        public GameObject GetRandom()
        {
            return Values[Random.Range(0, Values.Count)].BoardPrefab;
        }
    }
}
