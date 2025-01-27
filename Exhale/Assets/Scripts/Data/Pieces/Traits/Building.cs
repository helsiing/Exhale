using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exhale.Scripts.Data
{
    #region ECS
    public struct BuildingPlacementRequirementsDataBlob
    {
        public BlobArray<float2> Positions;
        public BlobArray<Entity> PieceEntities;
    }
    
    public struct BuildingComponentData : IComponentData
    {
        public BlobAssetReference<BuildingPlacementRequirementsDataBlob> PlacementRequirementsData;
    }
    #endregion
    
    [Serializable]
    public class BuildingPlacementRequirementsData
    {
        [SerializeField] private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        [SerializeField] private HexPieceTemplate pieceTemplate;
        public HexPieceTemplate PieceTemplate => pieceTemplate;
    }

    [Serializable]
    public class Building : PieceTrait
    {
        [SerializeField] private List<BuildingPlacementRequirementsData> placementRequirementsData = new();
        public List<BuildingPlacementRequirementsData> PlacementRequirementsData => placementRequirementsData;

        public override bool ValidateConfig()
        {
            if (placementRequirementsData.Count == 0)
            {
                Debug.LogError("Building trait has no unlock requirements");
                return false;
            }

            if (placementRequirementsData.Any(requirement => requirement.PositionIndex.Equals(float2.zero)))
            {
                Debug.LogError("Position (0, 0) is protected");
                return false;
            }

            return true;
        }
    }
}