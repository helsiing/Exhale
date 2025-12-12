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
        private bool canBuildOn;
        public bool CanBuildOn => canBuildOn;
        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            return true;
        }
    }
}