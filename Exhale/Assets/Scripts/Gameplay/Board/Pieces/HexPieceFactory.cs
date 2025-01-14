using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
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
        
        public static GameObject GetPiece (HexPieceTemplate pieceTemplate)
        {
            if (pieceTemplate.TryGetTrait(out BoardObject boardObject))
            {
                GameObject pieceGameObject = Object.Instantiate(boardObject.Prefab);
                pieceGameObject.AddComponent<HexPiece>();
                if(pieceTemplate.HasTrait<Ground>())
                {
                    pieceGameObject.AddComponent<GroundPieceSimulation>();
                    pieceGameObject.AddComponent<GroundPiecePresentation>();
                }
                else if(pieceTemplate.HasTrait<Building>())
                {
                    pieceGameObject.AddComponent<BuildingPieceSimulation>();
                    pieceGameObject.AddComponent<BuildingPiecePresentation>();
                }

                return pieceGameObject;
            }

            return null;
        }
        
    }
}