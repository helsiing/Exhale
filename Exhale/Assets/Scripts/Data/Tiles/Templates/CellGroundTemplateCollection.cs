using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create CellGroundTemplateCollection", fileName = "CellGroundTemplateCollection", order = 0)]
    public class CellGroundTemplateCollection : ScriptableObjectCollection<CellGroundTemplate>
    {
        [SerializeField] private CellGroundTemplate groundTemplate;
        public CellGroundTemplate GroundTemplate => groundTemplate;
        
        public GameObject GetRandom()
        {
            return Values[Random.Range(0, Values.Count)].BoardPrefab;
        }
    }
}
