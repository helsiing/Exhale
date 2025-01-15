using System;
using System.Collections.Generic;
using System.Linq;
using Data.Yield;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public class BuildingUnlockRequirementsData
    {
        [SerializeField] private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
            
        [SerializeField] private HexPieceTemplate pieceTemplate;
        public HexPieceTemplate PieceTemplate => pieceTemplate;
    }
    
    [Serializable]
    public class Building : PieceTrait
    {
        [SerializeField] private List<BuildingUnlockRequirementsData> unlockRequirementsData = new();
        public List<BuildingUnlockRequirementsData> UnlockRequirementsData => unlockRequirementsData;
        
        
        public override bool ValidateConfig()
        {
            if (unlockRequirementsData.Count == 0)
            {
                Debug.LogError("Building trait has no unlock requirements");
                return false;
            }

            if (unlockRequirementsData.Any(data => data.PositionIndex == Vector2.zero))
            {
                Debug.LogError("Position (0, 0) is protected");
                return false;
            }

            return true;
        }
    }
}