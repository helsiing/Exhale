using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exhale.Scripts.Data
{

    /// <summary>
    ///     Trait that indicates the board prefab that should be instantiated for a piece.
    /// </summary>
    [Serializable]
    public class BoardObject : PieceTrait
    {
        [Title("Board Object")]
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;

        public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
        {
            if(pieceTemplate.TryGetTrait(out BoardObject boardObject))
            {
                return boardObject.Prefab != null;
            }

            return false;
        }
    }
}