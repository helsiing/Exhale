using System.Linq;
using Exhale.Scripts.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    [RequireComponent(typeof(PieceAuthoring))]
    public class PieceBuildingAuthoring : MonoBehaviour
    {
        private PieceAuthoring GetPieceAuthoring()
        {
            return GetComponent<PieceAuthoring>();
        }

        private BlobAssetReference<BuildingPlacementRequirementsDataBlob> CreateBuildingDataBlob(float2[] positions,
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

        private class Baker : Baker<PieceBuildingAuthoring>
        {
            public override void Bake(PieceBuildingAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Renderable);

                HexPieceTemplate pieceTemplate = authoring.GetPieceAuthoring().PieceTemplate;

                if (!pieceTemplate.TryGetTrait(out Building building))
                {
                    Debug.Log($"Piece {pieceTemplate.name} does not have a Building trait");
                }
                
                var positions = building.PlacementRequirementsData
                    .Select(data => (float2)data.PositionIndex)
                    .ToArray();
                
                var entities = building.PlacementRequirementsData
                    .Select(data => GetEntity(data.PieceTemplate.GetPrefab(), TransformUsageFlags.Dynamic))
                    .ToArray();
                
                
                var blobAsset = authoring.CreateBuildingDataBlob(positions, entities);
                
                BuildingComponentData componentData = new BuildingComponentData
                {
                    PlacementRequirementsData = blobAsset
                };
                AddComponent(entity, componentData);
            }
        }
    }
}