using System;
using LBG;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    [SubclassPath("Gameplay", "Ground")]
    public class Ground : PieceTrait
    {
        [Title("Ground")]
        [SerializeField]
        private Yield yield;
        public Yield Yield => yield;
        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            return true;
        }
    }
}