using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create HexCellBuildingCollection", fileName = "HexCellBuildingCollection", order = 0)]
    public class HexCellBuildingCollection : ScriptableObjectCollection<HexCellBuilding>
    {
    }
}
