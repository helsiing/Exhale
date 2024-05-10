using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create CellElementCollection", fileName = "CellElementCollection", order = 0)]
    public class CellElementCollection : ScriptableObjectCollection<CellElement>
    {
    }
}
