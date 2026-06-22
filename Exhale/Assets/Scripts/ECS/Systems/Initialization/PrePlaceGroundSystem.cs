using System.Collections.Generic;
using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Scripts.Data;
using Exhale.Utils;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Exhale.ECS.Systems
{
    // One-shot: once the board tiles exist, guarantees a small connected ground patch at the
    // board centre (start tile + random neighbours, CenterPatchSize total), then scatters the
    // rest up to PrePlacedGroundCount on random tiles. Each tile is assigned a random
    // Ground-trait type from the baked GroundPieceId buffer.
    // Chosen tiles are marked occupied (so building placement requirements can read
    // OccupyingPieceId) and emit a PieceCreationRequest so PieceFactorySystem spawns the
    // visual — the same path a normal card placement uses. Tiles need not be adjacent;
    // occupying them unlocks their neighbours via EnableAdjacentTilesSystem as buildable spots.
    [UpdateAfter(typeof(BoardInitializationSystem))]
    public partial class PrePlaceGroundSystem : SystemBase
    {
        private bool done;

        protected override void OnCreate()
        {
            RequireForUpdate<BoardDataComponent>();
        }

        protected override void OnUpdate()
        {
            if (done) return;

            var board = SystemAPI.GetSingleton<BoardDataComponent>();
            if (board.PrePlacedGroundCount <= 0)
            {
                done = true;
                return;
            }

            // Baked list of ground piece ids to pick from. Copy out before any structural
            // change below — the DynamicBuffer handle is invalidated by CreateEntity/RemoveComponent.
            if (!SystemAPI.TryGetSingletonBuffer<GroundPieceId>(out var groundIdsBuffer, isReadOnly: true)
                || groundIdsBuffer.Length == 0)
            {
                Debug.LogWarning("[PrePlaceGroundSystem] No GroundPieceId entries baked — " +
                                 "no piece template has a Ground trait. Skipping pre-placement.");
                done = true;
                return;
            }

            var groundIds = new int[groundIdsBuffer.Length];
            for (int i = 0; i < groundIdsBuffer.Length; i++)
                groundIds[i] = groundIdsBuffer[i].Value;

            // Tiles are created via a deferred ECB in BoardInitializationSystem, so they may
            // not exist for the first frame(s). Gather every tile (incl. start-disabled ones)
            // into a flat list (for scatter) and a position lookup (for the centre patch).
            var tiles = new List<(int2 pos, Entity entity)>();
            var tileByPos = new Dictionary<int2, Entity>();
            foreach (var (tileData, entity) in SystemAPI
                .Query<RefRO<TileData>>()
                .WithEntityAccess()
                .WithOptions(EntityQueryOptions.IncludeDisabledEntities))
            {
                tiles.Add((tileData.ValueRO.PositionIndex, entity));
                tileByPos[tileData.ValueRO.PositionIndex] = entity;
            }

            if (tiles.Count == 0)
                return; // Board not spawned yet — try again next frame.

            var rng = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
            var placed = new HashSet<int2>();

            // Local placement helper. Captures rng by reference so its state advances.
            bool TryPlace(int2 pos, Entity tileEntity)
            {
                var tileData = EntityManager.GetComponentData<TileData>(tileEntity);
                if (tileData.IsOccupied) return false; // Already has a piece — don't clobber it.

                int groundId = groundIds[rng.NextInt(0, groundIds.Length)];

                tileData.IsEnabled = true;
                tileData.IsOccupied = true;
                tileData.OccupyingPieceId = groundId;
                EntityManager.SetComponentData(tileEntity, tileData);

                if (EntityManager.HasComponent<Disabled>(tileEntity))
                    EntityManager.RemoveComponent<Disabled>(tileEntity);

                var request = EntityManager.CreateEntity();
                EntityManager.AddComponentData(request, new PieceCreationRequest
                {
                    PositionIndex = pos,
                    PieceId       = groundId
                });

                placed.Add(pos);
                return true;
            }

            // --- Guaranteed centre patch: the start tile + (CenterPatchSize - 1) random neighbours. ---
            int2 center = board.StartPosition;
            if (tileByPos.TryGetValue(center, out var centerEntity))
                TryPlace(center, centerEntity);

            var neighbours = new List<int2>();
            for (int d = 0; d < BoardHelper.HexNeighborCount; d++)
            {
                var n = BoardHelper.GetHexNeighbor(center, d);
                if (tileByPos.ContainsKey(n) && !placed.Contains(n))
                    neighbours.Add(n);
            }
            // Shuffle neighbours and take enough to fill the patch.
            for (int i = 0; i < neighbours.Count && placed.Count < CenterPatchSize; i++)
            {
                int j = rng.NextInt(i, neighbours.Count);
                (neighbours[i], neighbours[j]) = (neighbours[j], neighbours[i]);
                var n = neighbours[i];
                TryPlace(n, tileByPos[n]);
            }

            // --- Scatter the rest randomly up to the configured total (patch counts toward it). ---
            int target = math.min(math.max(board.PrePlacedGroundCount, placed.Count), tiles.Count);
            for (int i = 0; i < tiles.Count && placed.Count < target; i++)
            {
                int j = rng.NextInt(i, tiles.Count);
                (tiles[i], tiles[j]) = (tiles[j], tiles[i]);

                var (pos, tileEntity) = tiles[i];
                if (placed.Contains(pos)) continue;
                TryPlace(pos, tileEntity);
            }

            done = true;
            Debug.Log($"[PrePlaceGroundSystem] Placed {placed.Count} random ground tiles " +
                      $"(incl. centre patch of up to {CenterPatchSize} around {center}) " +
                      $"from {groundIds.Length} ground type(s).");
        }

        private const int CenterPatchSize = 3;
    }
}
