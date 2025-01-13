using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObject Collection/Collections/Create HexTileElementsCollection", fileName = "HexTileElementsCollection", order = 0)]
    public class HexTileElementsCollection : ScriptableObjectCollection<HexTileElement>
    {
    }
}
