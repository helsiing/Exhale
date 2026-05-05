using System;
using Exhale.Scripts.Gameplay;
using LBG;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    [SubclassPath("Gameplay", "Yield")]
    public class Yield : PieceTrait
    {
        [SerializeField] private YieldTemplate yieldTemplate;

        [SerializeField] private int amount;
        public YieldTemplate YieldTemplate => yieldTemplate;
        public int Amount => amount;

        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            return true;
        }
    }
}