using System;
using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.Gameplay;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exhale.Scripts.Data
{
    #region Placement requirements
    public struct BuildingPlacementRequirementsDataBlob
    {
        public BlobArray<float2> Positions;
        public BlobArray<Entity> PieceEntities;
    }
    
    [Serializable]
    public class BuildingPlacementRequirementItemData
    {
        [SerializeField] private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        [SerializeField] private HexPieceTemplate pieceTemplate;
        public HexPieceTemplate PieceTemplate => pieceTemplate;
    }
    
    #endregion
    
    #region Cost
    public struct BuildingCostItemDataBlob
    {
        public BlobArray<int> YieldTemplateIds;
        public BlobArray<int> Costs;
    }
    
    [Serializable]
    public class BuildingCostItemData
    {
        [SerializeField] private int amount;
        public int Amount => amount;
        
        [SerializeField] private YieldTemplate yieldTemplate;
        public YieldTemplate YieldTemplate => yieldTemplate;
    }
    #endregion
    
    public struct BuildingComponentData : IComponentData
    {
        public BlobAssetReference<BuildingPlacementRequirementsDataBlob> PlacementRequirementsData;
        public BlobAssetReference<BuildingCostItemDataBlob> CostData;
    }

    [Serializable]
    public class Building : PieceTrait
    {
        [Space(20)]
        [SerializeField] private List<BuildingPlacementRequirementItemData> placementRequirementsData = new();
        public List<BuildingPlacementRequirementItemData> PlacementRequirementsData => placementRequirementsData;
        
        [Space(20)]
        [SerializeField] private List<BuildingCostItemData> costData = new();
        public List<BuildingCostItemData> CostData => costData;

        public static BlobAssetReference<BuildingPlacementRequirementsDataBlob> CreateBuildingPlacementRequirementDataBlob(float2[] positions,
            Entity[] entities)
        {
            // Ensure the input arrays match in length
            if (positions.Length != entities.Length)
            {
                Debug.LogError("Dimensions and HitPoints arrays must have the same length!");
                return default;
            }

            BlobBuilder builder = new(Allocator.Temp);

            // Construct the root blob
            ref BuildingPlacementRequirementsDataBlob root =
                ref builder.ConstructRoot<BuildingPlacementRequirementsDataBlob>();

            // Allocate the BlobArrays
            BlobBuilderArray<float2> positionsArray = builder.Allocate(ref root.Positions, positions.Length);
            BlobBuilderArray<Entity> entitiesArray = builder.Allocate(ref root.PieceEntities, entities.Length);

            // Fill the BlobArrays
            for (int i = 0; i < positions.Length; i++)
            {
                positionsArray[i] = positions[i];
                entitiesArray[i] = entities[i];
            }

            // Finalize and return the BlobAssetReference
            BlobAssetReference<BuildingPlacementRequirementsDataBlob> blobAsset =
                builder.CreateBlobAssetReference<BuildingPlacementRequirementsDataBlob>(Allocator.Persistent);
            builder.Dispose();

            return blobAsset;
        }
        
        public static BlobAssetReference<BuildingCostItemDataBlob> CreateBuildingCostDataBlob(int[] yieldTemplateIds,
            int[] costs)
        {
            // Ensure the input arrays match in length
            if (yieldTemplateIds.Length != costs.Length)
            {
                Debug.LogError("Dimensions and HitPoints arrays must have the same length!");
                return default;
            }

            BlobBuilder builder = new(Allocator.Temp);

            // Construct the root blob
            ref BuildingCostItemDataBlob root =
                ref builder.ConstructRoot<BuildingCostItemDataBlob>();

            // Allocate the BlobArrays
            BlobBuilderArray<int> yieldTemplateIdsArray = builder.Allocate(ref root.YieldTemplateIds, yieldTemplateIds.Length);
            BlobBuilderArray<int> costsArray = builder.Allocate(ref root.Costs, costs.Length);

            // Fill the BlobArrays
            for (int i = 0; i < yieldTemplateIds.Length; i++)
            {
                yieldTemplateIdsArray[i] = yieldTemplateIds[i];
                costsArray[i] = costs[i];
            }

            // Finalize and return the BlobAssetReference
            BlobAssetReference<BuildingCostItemDataBlob> blobAsset =
                builder.CreateBlobAssetReference<BuildingCostItemDataBlob>(Allocator.Persistent);
            builder.Dispose();

            return blobAsset;
        }
        
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
            
            //TODO: Validate cost data

            return true;
        }
    }
}