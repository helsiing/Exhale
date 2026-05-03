using System.Collections.Generic;
using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Services;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace ECS.Systems
{
    [UpdateAfter(typeof(TileHighlightSystem))]
    [UpdateBefore(typeof(PiecePlacementSystem))]
    public partial class TileArmingSystem : SystemBase
    {
        // Fallback colors used when no TileHighlightConfigAuthoring GO is in the subscene.
        private static readonly float4 DefaultArmedColor   = new(1.00f, 0.65f, 0.10f, 1f);
        private static readonly float4 DefaultPreviewColor = new(0.55f, 0.75f, 1.00f, 1f);

        private readonly ServiceReference<IPlacementService> placementService = new();

        private Entity armedTileEntity;
        private readonly List<Entity> adjacencyPreviewEntities = new();

        // Rebuilt on each arm — maps non-enabled tile positions to their entities
        // for neighbour lookup. Only populated on click, not every frame.
        private readonly Dictionary<int2, Entity> disabledTilesByPosition = new();

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            RequireForUpdate<PointerInputData>();
        }

        protected override void OnUpdate()
        {
            var service = placementService.Reference;
            if (service == null) return;

            // Cancel path: PlacementService returned to Idle (e.g. Esc pressed) without
            // going through TileArmingSystem — clear dangling tags one frame later.
            if (armedTileEntity != Entity.Null &&
                service.State is PlacementState.Idle or PlacementState.TileHovered)
            {
                ClearArmedState();
            }

            PointerInputData inputData = SystemAPI.GetSingleton<PointerInputData>();
            if (!inputData.IsValid || !inputData.IsClickDown) return;

            PhysicsWorldSingleton physics = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            RaycastInput rayInput = new()
            {
                Start  = inputData.RayOrigin,
                End    = inputData.RayOrigin + inputData.RayDirection * 1000f,
                Filter = CollisionFilter.Default
            };

            if (!physics.PhysicsWorld.CollisionWorld.CastRay(rayInput, out RaycastHit hit)) return;

            Entity hitEntity = physics.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            if (!SystemAPI.HasComponent<TileData>(hitEntity)) return;

            TileData tileData = SystemAPI.GetComponent<TileData>(hitEntity);
            if (!tileData.IsEnabled || tileData.IsOccupied)
            {
                Debug.Log($"[TileArmingSystem] Clicked tile {tileData.PositionIndex} — IsEnabled={tileData.IsEnabled} IsOccupied={tileData.IsOccupied}. Ignoring.");
                return;
            }

            // Disarm the previous tile before arming the new one.
            ClearArmedState();

            if (!service.TryArmTile(tileData.PositionIndex, hitEntity)) return;

            // Read colors from the designer-facing authoring config, or fall back to defaults.
            float4 armedColor   = DefaultArmedColor;
            float4 previewColor = DefaultPreviewColor;
            if (SystemAPI.HasSingleton<TileHighlightConfigData>())
            {
                var cfg  = SystemAPI.GetSingleton<TileHighlightConfigData>();
                armedColor   = cfg.ArmedColor;
                previewColor = cfg.PreviewColor;
            }

            // Tag the armed tile, flip its highlight bool, and tint it.
            armedTileEntity = hitEntity;
            EntityManager.AddComponent<TileArmedTag>(hitEntity);
            SetHighlightArmed(hitEntity, true);
            SetColorOverride(hitEntity, armedColor);

            // Tag disabled neighbours for the adjacency preview.
            BuildDisabledTileMap();
            foreach (var neighborPos in GetAdjacentPositions(tileData.PositionIndex))
            {
                if (!disabledTilesByPosition.TryGetValue(neighborPos, out Entity neighborEntity))
                    continue;

                EntityManager.AddComponent<TileAdjacencyPreviewTag>(neighborEntity);
                SetHighlightAdjacencyPreview(neighborEntity, true);
                SetColorOverride(neighborEntity, previewColor);
                adjacencyPreviewEntities.Add(neighborEntity);
            }
        }

        private void ClearArmedState()
        {
            if (armedTileEntity != Entity.Null)
            {
                if (EntityManager.HasComponent<TileArmedTag>(armedTileEntity))
                    EntityManager.RemoveComponent<TileArmedTag>(armedTileEntity);
                SetHighlightArmed(armedTileEntity, false);
                ClearColorOverride(armedTileEntity);
                armedTileEntity = Entity.Null;
            }

            foreach (var entity in adjacencyPreviewEntities)
            {
                if (EntityManager.HasComponent<TileAdjacencyPreviewTag>(entity))
                    EntityManager.RemoveComponent<TileAdjacencyPreviewTag>(entity);
                SetHighlightAdjacencyPreview(entity, false);
                ClearColorOverride(entity);
            }
            adjacencyPreviewEntities.Clear();
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

        private void SetHighlightArmed(Entity entity, bool value)
        {
            if (!EntityManager.HasComponent<TileDataHighlight>(entity)) return;
            var h = EntityManager.GetComponentData<TileDataHighlight>(entity);
            h.IsArmed = value;
            EntityManager.SetComponentData(entity, h);
        }

        private void SetHighlightAdjacencyPreview(Entity entity, bool value)
        {
            if (!EntityManager.HasComponent<TileDataHighlight>(entity)) return;
            var h = EntityManager.GetComponentData<TileDataHighlight>(entity);
            h.IsAdjacencyPreview = value;
            EntityManager.SetComponentData(entity, h);
        }

        // Builds a position → entity map for tiles not yet enabled (the "would unlock" set).
        // Called only on click, so the cost of iterating all tiles is acceptable.
        private void BuildDisabledTileMap()
        {
            disabledTilesByPosition.Clear();
            foreach (var (tileData, entity) in SystemAPI.Query<RefRO<TileData>>().WithEntityAccess())
            {
                if (!tileData.ValueRO.IsEnabled)
                    disabledTilesByPosition[tileData.ValueRO.PositionIndex] = entity;
            }
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
