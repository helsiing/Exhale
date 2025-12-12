using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
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