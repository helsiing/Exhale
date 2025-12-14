using System;
using LBG;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    /// <summary>
    ///     Trait that indicates the board prefab that should be instantiated for a card.
    /// </summary>
    [Serializable]
    [SubclassPath("Visuals", "Card Object")]
    public class CardObject : PieceTrait
    {
        [Title("Board Object")]
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;

        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            if(pieceTemplate.TryGetTrait(out CardObject boardObject))
            {
                return boardObject.Prefab != null;
            }

            return false;
        }
    }
}