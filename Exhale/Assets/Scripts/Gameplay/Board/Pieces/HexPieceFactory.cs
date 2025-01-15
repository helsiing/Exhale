using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Gameplay
{
    public static class HexPieceFactory
    {
        public static HexPieceTemplate GetRandomTemplate ()
        {
            return HexPieceTemplateCollection.GetRandomTemplate();
        }
        
        public static HexPieceTemplate GetRandomTemplate<T> () where T : PieceTrait
        {
            return HexPieceTemplateCollection.GetRandomTemplate<T>();
        }
        
        public static GameObject GetPiece (HexPieceTemplate pieceTemplate, bool shouldInitialize = true)
        {
            if (!pieceTemplate.TryGetTrait(out BoardObject boardObject)) return null;
            
            GameObject pieceGameObject = Object.Instantiate(boardObject.Prefab);
            if (!shouldInitialize) return pieceGameObject;
                
            pieceGameObject.AddComponent<HexPiece>();
            if (pieceTemplate.HasTrait<Ground>())
            {
                pieceGameObject.AddComponent<GroundPieceSimulation>();
                pieceGameObject.AddComponent<GroundPiecePresentation>();
            }
            else if (pieceTemplate.HasTrait<Building>())
            {
                pieceGameObject.AddComponent<BuildingPieceSimulation>();
                pieceGameObject.AddComponent<BuildingPiecePresentation>();
            }

            return pieceGameObject;

        }
        
    }
}