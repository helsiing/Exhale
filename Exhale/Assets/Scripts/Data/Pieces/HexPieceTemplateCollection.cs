using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "Exhale/Data/HexPieceTemplateCollection", fileName = "HexPieceTemplateCollection", order = 0)]
    public class HexPieceTemplateCollection : ScriptableObjectCollection<HexPieceTemplate>
    {
        public static HexPieceTemplate GetRandomTemplate()
        {
            int count = Values.Count;
            return Values[Random.Range(0, count)];
        }
        
        public static HexPieceTemplate GetRandomTemplate<T> (T trait = null) where T : PieceTrait
        {
            List<HexPieceTemplate> templatesWithTrait = Values.Where(x => x.HasTrait<T>()).ToList();
            int count = templatesWithTrait.Count;
            return templatesWithTrait[Random.Range(0, count)];
        }
    }
}
