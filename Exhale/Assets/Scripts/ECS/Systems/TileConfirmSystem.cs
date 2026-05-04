using System.Collections.Generic;
using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Services;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using RaycastHit = Unity.Physics.RaycastHit;

namespace ECS.Systems
{
    [UpdateAfter(typeof(TileHighlightSystem))]
    [UpdateBefore(typeof(PiecePlacementSystem))]
    public partial class TileConfirmSystem : SystemBase
    {
        private static readonly float4 DefaultPreviewColor = new(0.55f, 0.75f, 1.00f, 1f);

        private readonly ServiceReference<IPlacementService> placementService = new();
        private readonly List<Entity> adjacencyPreviewEntities = new();
        private readonly Dictionary<int2, Entity> disabledTilesByPosition = new();
        private Entity hoveredTileForPreview = Entity.Null;

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            RequireForUpdate<PointerInputData>();
        }

        protected override void OnUpdate()
        {
            var service = placementService.Reference;
            if (service == null) return;

            // State left CardSelected (cancel or confirmed) — clear any lingering preview.
            if (adjacencyPreviewEntities.Count > 0 && service.State != PlacementState.CardSelected)
            {
                ClearAdjacencyPreview();
                hoveredTileForPreview = Entity.Null;
                return;
            }

            if (service.State != PlacementState.CardSelected) return;

            var inputData = SystemAPI.GetSingleton<PointerInputData>();
            if (!inputData.IsValid) return;

            var physics = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var rayInput = new RaycastInput
            {
                Start  = inputData.RayOrigin,
                End    = inputData.RayOrigin + inputData.RayDirection * 1000f,
                Filter = CollisionFilter.Default
            };

            if (!physics.PhysicsWorld.CollisionWorld.CastRay(rayInput, out RaycastHit hit))
            {
                ClearPreviewIfNeeded();
                return;
            }

            Entity hitEntity = physics.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            if (!SystemAPI.HasComponent<TileData>(hitEntity))
            {
                ClearPreviewIfNeeded();
                return;
            }

            // Click on a valid tile → confirm placement.
            if (inputData.IsClickDown)
            {
                if (!SystemAPI.HasComponent<TileValidForPlacementTag>(hitEntity)) return;

                var tileData = SystemAPI.GetComponent<TileData>(hitEntity);
                ClearAdjacencyPreview();
                hoveredTileForPreview = Entity.Null;
                service.TryConfirmTile(tileData.PositionIndex, hitEntity);
                return;
            }

            // Hovering over an invalid tile — clear preview and bail.
            if (!SystemAPI.HasComponent<TileValidForPlacementTag>(hitEntity))
            {
                ClearPreviewIfNeeded();
                return;
            }

            // Same valid tile as last frame — nothing to update.
            if (hitEntity == hoveredTileForPreview) return;

            // New valid tile hovered — show which disabled neighbours would unlock.
            ClearAdjacencyPreview();
            hoveredTileForPreview = hitEntity;

            float4 previewColor = DefaultPreviewColor;
            if (SystemAPI.HasSingleton<TileHighlightConfigData>())
                previewColor = SystemAPI.GetSingleton<TileHighlightConfigData>().PreviewColor;

            var hoveredTileData = SystemAPI.GetComponent<TileData>(hitEntity);
            BuildDisabledTileMap();

            foreach (var neighborPos in GetAdjacentPositions(hoveredTileData.PositionIndex))
            {
                if (!disabledTilesByPosition.TryGetValue(neighborPos, out Entity neighborEntity)) continue;

                EntityManager.AddComponent<TileAdjacencyPreviewTag>(neighborEntity);
                SetHighlightAdjacencyPreview(neighborEntity, true);
                SetColorOverride(neighborEntity, previewColor);
                adjacencyPreviewEntities.Add(neighborEntity);
            }
        }

        private void ClearPreviewIfNeeded()
        {
            if (hoveredTileForPreview == Entity.Null) return;
            ClearAdjacencyPreview();
            hoveredTileForPreview = Entity.Null;
        }

        private void ClearAdjacencyPreview()
        {
            foreach (var entity in adjacencyPreviewEntities)
            {
                if (EntityManager.HasComponent<TileAdjacencyPreviewTag>(entity))
                    EntityManager.RemoveComponent<TileAdjacencyPreviewTag>(entity);
                SetHighlightAdjacencyPreview(entity, false);
                ClearColorOverride(entity);
            }
            adjacencyPreviewEntities.Clear();
        }

        // WithAll<Disabled> is required — SystemAPI.Query excludes disabled entities by default.
        private void BuildDisabledTileMap()
        {
            disabledTilesByPosition.Clear();
            foreach (var (tileData, entity) in SystemAPI
                .Query<RefRO<TileData>>()
                .WithAll<Disabled>()
                .WithEntityAccess())
            {
                disabledTilesByPosition[tileData.ValueRO.PositionIndex] = entity;
            }
        }

        private void SetColorOverride(Entity entity, float4 color)
        {
            if (EntityManager.HasComponent<TileHighlightColorOverride>(entity))
                EntityManager.SetComponentData(entity, new TileHighlightColorOverride { Value = color });
            else
                EntityManager.AddComponentData(entity, new TileHighlightColorOverride { Value = color });
        }

        private void ClearColorOverride(Entity entity)
        {
            if (EntityManager.HasComponent<TileHighlightColorOverride>(entity))
                EntityManager.RemoveComponent<TileHighlightColorOverride>(entity);
        }

        private void SetHighlightAdjacencyPreview(Entity entity, bool value)
        {
            if (!EntityManager.HasComponent<TileDataHighlight>(entity)) return;
            var h = EntityManager.GetComponentData<TileDataHighlight>(entity);
            h.IsAdjacencyPreview = value;
            EntityManager.SetComponentData(entity, h);
        }

        // Axial hex neighbour offsets — must match EnableAdjacentTilesSystem.GetAdjacentPositions.
        private static int2[] GetAdjacentPositions(int2 p) => new[]
        {
            new int2(p.x + 1, p.y),
            new int2(p.x - 1, p.y),
            new int2(p.x,     p.y + 1),
            new int2(p.x,     p.y - 1),
            new int2(p.x + 1, p.y - 1),
            new int2(p.x - 1, p.y + 1),
        };
    }
}
