using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public class Building : PieceTrait
    {
        [Serializable]
        public class BuildingConstructionData
        {
            [SerializeField] private Vector2 positionIndex;
            private Vector2 PositionIndex => positionIndex;
            
            [SerializeField] private HexPieceTemplate pieceTemplate;
            private HexPieceTemplate PieceTemplate => pieceTemplate;
        }
        
        [SerializeField] private List<BuildingConstructionData> constructionRequirements = new List<BuildingConstructionData>();
        
        public override bool ValidateConfig()
        {
            if (constructionRequirements.Count == 0)
            {
                Debug.LogError("Building trait has no construction requirements");
                return false;
            }

            return true;
        }
    }
}