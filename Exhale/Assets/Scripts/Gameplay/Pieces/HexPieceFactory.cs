using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public static class HexPieceFactory
    {
        public static HexPieceTemplate GetRandomTemplate()
        {
            List<HexPieceTemplate> pieces = HexPieceTemplateCollection.Values.ToList();
            int count = pieces.Count;
            return pieces[Random.Range(0, count)];
        }

        public static GameObject GetRandomPiece(bool shouldInstantiate)
        {
            List<HexPieceTemplate> pieces = HexPieceTemplateCollection.Values.ToList();
            int count = pieces.Count;
            var randomPieceTemplate = pieces[Random.Range(0, count)];
            if(randomPieceTemplate.TryGetTrait(out BoardObject boardObject))
            {
                return shouldInstantiate ? Object.Instantiate(boardObject.Prefab) : boardObject.Prefab;
            }

            return null;
        }
        
    }
}