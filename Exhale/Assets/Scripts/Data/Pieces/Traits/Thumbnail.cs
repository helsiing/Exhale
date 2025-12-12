using System;
using LBG;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    /// <summary>
    ///     Trait that indicates the 2D thumbnail sprite for a piece.
    /// </summary>
    [Serializable]
    [SubclassPath("Visuals", "Thumbnail")]

    public class Thumbnail : PieceTrait
    {
        [Title("Thumbnail")]
        [SerializeField] private Sprite sprite;
        public Sprite Sprite => sprite;

        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            if(pieceTemplate.TryGetTrait(out Thumbnail thumbnail))
            {
                return thumbnail.sprite != null;
            }

            return false;
        }
    }
}