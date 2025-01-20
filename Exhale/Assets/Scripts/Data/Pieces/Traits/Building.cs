using System;
using System.Collections.Generic;
using System.Linq;
using Data.Yield;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public struct BuildingUnlockRequirementComponent : IComponentData
    {
        public float2 PositionIndex;
        public Entity pieceTemplateEntity;
    }
        
    [Serializable]
    public class BuildingUnlockRequirementsData
    {
        [InlineProperty, HideLabel, SerializeField]
        private BuildingUnlockRequirementComponent data;
        public BuildingUnlockRequirementComponent Data => data;

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

            if (unlockRequirementsData.Any(requirement => requirement.Data.PositionIndex.Equals(float2.zero)))
            {
                Debug.LogError("Position (0, 0) is protected");
                return false;
            }

            return true;
        }
    }
}