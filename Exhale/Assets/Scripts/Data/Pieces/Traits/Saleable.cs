using System;
using System.Linq;
using Exhale.Collections;
using LBG;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    [SubclassPath("Economy", "Saleable")]

    public sealed class Saleable : PieceTrait
    {
        [Title("Saleable")]
        [SerializeReference]
        private DictionaryWrapper<Yield, int> basePrice;
        public DictionaryWrapper<Yield, int> BasePrice => basePrice;
		
        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            return BasePrice.PairsInOrder.All(entry => entry.Key != null);
        }
    }
}