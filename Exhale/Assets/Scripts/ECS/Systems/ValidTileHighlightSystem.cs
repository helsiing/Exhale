using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Services;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Systems
{
    // Runs before TileHighlightSystem so the tags are in place when that system reads them.
    [UpdateBefore(typeof(global::ECS.Systems.TileHighlightSystem))]
    public partial class ValidTileHighlightSystem : SystemBase
    {
        private static readonly float4 DefaultValidColor = new(0.40f, 0.90f, 0.40f, 1f);

        private readonly ServiceReference<IPlacementService> placementService = new();
        private PlacementState previousState = PlacementState.Idle;

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

            float4 color = DefaultValidColor;
            if (SystemAPI.HasSingleton<TileHighlightConfigData>())
                color = SystemAPI.GetSingleton<TileHighlightConfigData>().ValidPlacementColor;

            // Collect entities first — structural changes cannot happen during iteration.
            var toTag = new NativeList<Entity>(Allocator.Temp);
            foreach (var (tileData, entity) in SystemAPI
                .Query<RefRO<TileData>>()
                .WithNone<TileValidForPlacementTag>()
                .WithEntityAccess())
            {
                if (!tileData.ValueRO.IsEnabled || tileData.ValueRO.IsOccupied) continue;
                if (!service.IsCardValidForTile(card, tileData.ValueRO.PositionIndex)) continue;
                toTag.Add(entity);
            }

            foreach (var entity in toTag)
            {
                EntityManager.AddComponent<TileValidForPlacementTag>(entity);
                if (EntityManager.HasComponent<TileHighlightColorOverride>(entity))
                    EntityManager.SetComponentData(entity, new TileHighlightColorOverride { Value = color });
                else
                    EntityManager.AddComponentData(entity, new TileHighlightColorOverride { Value = color });
            }

            toTag.Dispose();
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
                if (EntityManager.HasComponent<TileHighlightColorOverride>(entity))
                    EntityManager.RemoveComponent<TileHighlightColorOverride>(entity);
            }

            toClear.Dispose();
        }
    }
}
