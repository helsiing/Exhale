using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "Exhale/Data/HexPieceTemplateCollection", fileName = "HexPieceTemplateCollection", order = 0)]
    public class HexPieceTemplateCollection : ScriptableObjectCollection<HexPieceTemplate>
    {
    }
}
