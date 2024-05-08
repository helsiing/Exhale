using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create HexCellTypeCollection", fileName = "HexCellTypeCollection", order = 0)]
    public class HexCellTypeCollection : ScriptableObjectCollection<HexCellType>
    {
    }
}
