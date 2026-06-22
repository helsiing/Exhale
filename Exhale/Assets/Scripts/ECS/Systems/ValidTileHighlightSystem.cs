using System.Collections.Generic;
using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Exhale.ECS.Systems
{
    // Runs before TileHighlightSystem so the tags are in place when that system reads them.
    [UpdateBefore(typeof(global::ECS.Systems.TileHighlightSystem))]
    public partial class ValidTileHighlightSystem : SystemBase
    {
        private static readonly float4 DefaultValidColor      = new(0.40f, 0.90f, 0.40f, 0.1f);
        private static readonly float4 DefaultInvalidReqColor = new(0.90f, 0.30f, 0.30f, 0.1f);

        private readonly ServiceReference<IPlacementService> placementService = new();
        private PlacementState previousState = PlacementState.Idle;

        // Tracks root entities that received the invalid-requirements color for cleanup.
        private readonly List<Entity> invalidRequirementRoots = new();

        // Reused buffer to avoid per-call allocation in GetRenderingTargets.
        private readonly List<Entity> renderingTargetBuffer = new();

        protected override void OnUpdate()
        {
            var service = placementService.Reference;
            if (service == null) return;

            var current = service.State;

            if (current == PlacementState.CardSelected && previousState != PlacementState.CardSelected)
                TagValidTiles(service);
            else if (current != PlacementState.CardSelected && previousState == PlacementState.CardSelected)
                ClearValidTileTags();

            previousState = current;
        }

        private void TagValidTiles(IPlacementService service)
        {
            var card = service.SelectedCard;
            if (card == null) return;

            float4 validColor   = DefaultValidColor;
            float4 invalidColor = DefaultInvalidReqColor;
            if (SystemAPI.HasSingleton<TileHighlightConfigData>())
            {
                var cfg = SystemAPI.GetSingleton<TileHighlightConfigData>();
                validColor   = cfg.ValidPlacementColor;
                invalidColor = cfg.InvalidRequirementsColor;
            }

            var occupiedMap = BuildOccupiedMap();

            card.Template.TryGetTrait<Building>(out var buildingTrait);
            var requirements = buildingTrait?.PlacementRequirementsData;
            bool hasRequirements = requirements is { Count: > 0 };

            var toTagValid   = new NativeList<Entity>(Allocator.Temp);
            var toTagInvalid = new NativeList<Entity>(Allocator.Temp);

            foreach (var (tileData, entity) in SystemAPI
                .Query<RefRO<TileData>>()
                .WithNone<TileValidForPlacementTag>()
                .WithEntityAccess())
            {
                if (!tileData.ValueRO.IsEnabled || tileData.ValueRO.IsOccupied) continue;
                if (!service.IsCardValidForTile(card, tileData.ValueRO.PositionIndex)) continue;

                if (hasRequirements && !AreRequirementsMet(tileData.ValueRO.PositionIndex, requirements, occupiedMap))
                    toTagInvalid.Add(entity);
                else
                    toTagValid.Add(entity);
            }

            foreach (var entity in toTagValid)
            {
                EntityManager.AddComponent<TileValidForPlacementTag>(entity);
                SetColorOverride(entity, validColor);
            }

            foreach (var entity in toTagInvalid)
            {
                invalidRequirementRoots.Add(entity);
                SetColorOverride(entity, invalidColor);
            }

            toTagValid.Dispose();
            toTagInvalid.Dispose();
            occupiedMap.Dispose();
        }

        private void ClearValidTileTags()
        {
            var toClear = new NativeList<Entity>(Allocator.Temp);
            foreach (var (_, entity) in SystemAPI
                .Query<RefRO<TileValidForPlacementTag>>()
                .WithEntityAccess())
            {
                toClear.Add(entity);
            }

            foreach (var entity in toClear)
            {
                EntityManager.RemoveComponent<TileValidForPlacementTag>(entity);
                ClearColorOverride(entity);
            }

            toClear.Dispose();

            foreach (var root in invalidRequirementRoots)
                ClearColorOverride(root);
            invalidRequirementRoots.Clear();
        }

        private static bool AreRequirementsMet(
            int2 tilePos,
            List<BuildingPlacementRequirementItemData> requirements,
            NativeHashMap<int2, int> occupiedMap)
        {
            foreach (var req in requirements)
            {
                var targetPos = tilePos + (int2)math.round((float2)(Vector2)req.PositionIndex);
                if (!occupiedMap.TryGetValue(targetPos, out int presentId)) return false;
                if (presentId != req.PieceTemplate.GetId()) return false;
            }
            return true;
        }

        private NativeHashMap<int2, int> BuildOccupiedMap()
        {
            var map = new NativeHashMap<int2, int>(64, Allocator.Temp);
            foreach (var tileData in SystemAPI.Query<RefRO<TileData>>())
            {
                if (!tileData.ValueRO.IsOccupied) continue;
                map.TryAdd(tileData.ValueRO.PositionIndex, tileData.ValueRO.OccupyingPieceId);
            }
            return map;
        }

        // Applies color to the entity's rendering children (via LinkedEntityGroup/MaterialMeshInfo),
        // falling back to the entity itself if no rendering children are found.
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

        // Fills 'results' with child entities that have MaterialMeshInfo (the actual renderers).
        // Falls back to the root itself when no rendering children are found.
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
