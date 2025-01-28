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

                BuildingComponentData componentData = new()
                {
                    PlacementRequirementsData = GetPlacementRequirementsBlob(building),
                    CostData = GetCostBlob(building)
                };
                AddComponent(entity, componentData);
            }

            private BlobAssetReference<BuildingPlacementRequirementsDataBlob> GetPlacementRequirementsBlob(Building building)
            {
                float2[] positions = building.PlacementRequirementsData
                    .Select(data => (float2) data.PositionIndex)
                    .ToArray();

                Entity[] entities = building.PlacementRequirementsData
                    .Select(data => GetEntity(data.PieceTemplate.GetPrefab(), TransformUsageFlags.Dynamic))
                    .ToArray();

                return Building.CreateBuildingPlacementRequirementDataBlob(positions, entities);
            }
            
            private BlobAssetReference<BuildingCostItemDataBlob> GetCostBlob(Building building)
            {
                int[] yieldTemplatesIds = building.CostData
                    .Select(data => data.YieldTemplate.GetId())
                    .ToArray();

                int[] cost = building.CostData
                    .Select(data => data.Amount)
                    .ToArray();

                return Building.CreateBuildingCostDataBlob(yieldTemplatesIds, cost);
            }
        }
    }
}