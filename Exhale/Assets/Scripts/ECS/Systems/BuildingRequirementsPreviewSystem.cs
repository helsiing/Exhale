using System.Collections.Generic;
using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace ECS.Systems
{
    [UpdateAfter(typeof(TileConfirmSystem))]
    [UpdateBefore(typeof(PiecePlacementSystem))]
    public partial class BuildingRequirementsPreviewSystem : SystemBase
    {
        private static readonly float4 DefaultSatisfiedColor = new(0.30f, 0.85f, 0.75f, .25f);
        private static readonly float4 DefaultMissingColor   = new(0.95f, 0.25f, 0.25f, .25f);

        private readonly ServiceReference<IPlacementService> placementService = new();

        // Root entities whose rendering children received a color override this frame.
        private readonly List<Entity> previewRoots = new();
        private readonly List<Entity> renderingTargetBuffer = new();
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

            if (previewRoots.Count > 0 && service.State != PlacementState.CardSelected)
            {
                ClearRequirementsPreview();
                hoveredTileForPreview = Entity.Null;
                return;
            }

            if (service.State != PlacementState.CardSelected) return;

            var card = service.SelectedCard;
            if (card == null) return;

            if (!card.Template.TryGetTrait<Building>(out var buildingTrait)) return;
            var requirements = buildingTrait.PlacementRequirementsData;
            if (requirements == null || requirements.Count == 0) return;

            var inputData = SystemAPI.GetSingleton<PointerInputData>();
            if (!inputData.IsValid)
            {
                ClearPreviewIfNeeded();
                return;
            }

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

            var hitTileData = SystemAPI.GetComponent<TileData>(hitEntity);
            if (!hitTileData.IsEnabled || hitTileData.IsOccupied)
            {
                ClearPreviewIfNeeded();
                return;
            }

            if (hitEntity == hoveredTileForPreview) return;

            ClearRequirementsPreview();
            hoveredTileForPreview = hitEntity;

            float4 satisfiedColor = DefaultSatisfiedColor;
            float4 missingColor   = DefaultMissingColor;
            if (SystemAPI.HasSingleton<TileHighlightConfigData>())
            {
                var cfg = SystemAPI.GetSingleton<TileHighlightConfigData>();
                satisfiedColor = cfg.RequirementSatisfiedColor;
                missingColor   = cfg.RequirementMissingColor;
            }

            // Build maps for O(n) lookup: position → tile entity and position → piece entity.
            var tileEntityMap  = new NativeHashMap<int2, Entity>(64, Allocator.Temp);
            var tileOccupyMap  = new NativeHashMap<int2, int>(64, Allocator.Temp);
            var pieceEntityMap = new NativeHashMap<int2, Entity>(32, Allocator.Temp);

            foreach (var (td, e) in SystemAPI.Query<RefRO<TileData>>().WithEntityAccess())
            {
                tileEntityMap.TryAdd(td.ValueRO.PositionIndex, e);
                tileOccupyMap.TryAdd(td.ValueRO.PositionIndex, td.ValueRO.OccupyingPieceId);
            }

            foreach (var (bp, e) in SystemAPI.Query<RefRO<BoardPosition>>().WithEntityAccess())
                pieceEntityMap.TryAdd(bp.ValueRO.PositionIndex, e);

            foreach (var req in requirements)
            {
                var targetPos = hitTileData.PositionIndex + (int2)math.round((float2)(Vector2)req.PositionIndex);

                if (!tileEntityMap.TryGetValue(targetPos, out Entity tileEntity)) continue;

                tileOccupyMap.TryGetValue(targetPos, out int occupyingId);
                bool satisfied = occupyingId == req.PieceTemplate.GetId();
                float4 color = satisfied ? satisfiedColor : missingColor;

                // Tint the tile entity (visible around the edges of any placed piece).
                SetColorOverride(tileEntity, color);
                previewRoots.Add(tileEntity);

                // Also tint the placed piece entity at this position for clearer feedback.
                if (pieceEntityMap.TryGetValue(targetPos, out Entity pieceEntity))
                {
                    SetColorOverride(pieceEntity, color);
                    previewRoots.Add(pieceEntity);
                }
            }

            tileEntityMap.Dispose();
            tileOccupyMap.Dispose();
            pieceEntityMap.Dispose();
        }

        private void ClearPreviewIfNeeded()
        {
            if (hoveredTileForPreview == Entity.Null) return;
            ClearRequirementsPreview();
            hoveredTileForPreview = Entity.Null;
        }

        private void ClearRequirementsPreview()
        {
            foreach (var root in previewRoots)
                ClearColorOverride(root);
            previewRoots.Clear();
        }

        private void SetColorOverride(Entity rootEntity, float4 color)
        {
            GetRenderingTargets(rootEntity, renderingTargetBuffer);
            foreach (var e in renderingTargetBuffer)
            {
                if (EntityManager.HasComponent<TileHighlightColorOverride>(e))
                    EntityManager.SetComponentData(e, new TileHighlightColorOverride { Value = color });
                else
                    EntityManager.AddComponentData(e, new TileHighlightColorOverride { Value = color });
            }
        }

        private void ClearColorOverride(Entity rootEntity)
        {
            GetRenderingTargets(rootEntity, renderingTargetBuffer);
            foreach (var e in renderingTargetBuffer)
            {
                if (EntityManager.HasComponent<TileHighlightColorOverride>(e))
                    EntityManager.RemoveComponent<TileHighlightColorOverride>(e);
            }
        }

        private void GetRenderingTargets(Entity rootEntity, List<Entity> results)
        {
            results.Clear();
            if (EntityManager.HasBuffer<LinkedEntityGroup>(rootEntity))
            {
                var group = EntityManager.GetBuffer<LinkedEntityGroup>(rootEntity, true);
                for (int i = 0; i < group.Length; i++)
                {
                    var child = group[i].Value;
                    if (EntityManager.HasComponent<MaterialMeshInfo>(child))
                        results.Add(child);
                }
            }
            if (results.Count == 0)
                results.Add(rootEntity);
        }
    }
}
