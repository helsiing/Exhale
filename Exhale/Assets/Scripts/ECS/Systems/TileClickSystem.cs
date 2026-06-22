using System.Collections.Generic;
using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Systems
{
    // Renamed from TileClickSystem. No longer responds to mouse clicks directly —
    // piece spawning is now triggered by IPlacementService.OnCardLanded so the full
    // tile-first placement flow (arm → card launch → land) drives creation.
    public partial class PiecePlacementSystem : SystemBase
    {
        private readonly ServiceReference<IPlacementService> placementService = new();
        private readonly Queue<(HandCard card, int2 pos)> pending = new();
        private bool subscribed;

        protected override void OnUpdate()
        {
            // Subscribe lazily — services are registered in MonoBehaviour.Awake which
            // may run after the ECS world is created.
            if (!subscribed && placementService.Reference != null)
            {
                placementService.Reference.OnCardLanded += OnCardLanded;
                subscribed = true;
            }

            while (pending.Count > 0)
            {
                var (card, pos) = pending.Dequeue();
                Debug.Log($"[PiecePlacementSystem] Processing placement: card={card.Template.name} id={card.Template.GetId()} pos={pos}");

                bool tileFound = false;
                foreach (var tileData in SystemAPI.Query<RefRW<TileData>>())
                {
                    if (!tileData.ValueRO.PositionIndex.Equals(pos)) continue;
                    tileData.ValueRW.IsOccupied = true;
                    tileData.ValueRW.OccupyingPieceId = card.Template.GetId();
                    tileFound = true;
                    break;
                }
                Debug.Log($"[PiecePlacementSystem] Tile found and marked occupied: {tileFound}");

                var request = EntityManager.CreateEntity();
                EntityManager.AddComponentData(request, new PieceCreationRequest
                {
                    PositionIndex = pos,
                    PieceId       = card.Template.GetId()
                });
                Debug.Log($"[PiecePlacementSystem] PieceCreationRequest created for pieceId={card.Template.GetId()}");
            }
        }

        protected override void OnDestroy()
        {
            if (subscribed && placementService.Reference != null)
                placementService.Reference.OnCardLanded -= OnCardLanded;
        }

        // Called on the main thread from PlacementService.NotifyCardLanded().
        private void OnCardLanded(HandCard card, int2 pos) =>
            pending.Enqueue((card, pos));
    }
}
