using System;
using System.Collections.Generic;
using Data.Yield;
using LBG;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    [SubclassPath("Gameplay", "Yield")]
    public class Yield : PieceTrait
    {
        [SerializeField] private List<YieldData> yields = new();
        public List<YieldData> Yields => yields;

        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            return true;
        }
    }
}