using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create HexCellGroundTemplateCollection", fileName = "HexCellGroundTemplateCollection", order = 0)]
    public class HexCellGroundTemplateCollection : ScriptableObjectCollection<HexCellGroundTemplate>
    {
        [SerializeField] private HexCellGroundTemplate groundTemplate;
        public HexCellGroundTemplate GroundTemplate => groundTemplate;
    }
}
