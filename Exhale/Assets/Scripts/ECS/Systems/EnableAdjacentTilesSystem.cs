using Exhale.ECS.Authoring;
using Exhale.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
    [BurstCompile]
    public partial struct EnableAdjacentTilesSystem : ISystem
    {
        private NativeHashMap<int2, Entity> tileEntityMap;
        private EntityQuery changedTilesQuery;

        // OnCreate is intentionally NOT [BurstCompile] — AddChangedVersionFilter is a managed call.
        public void OnCreate(ref SystemState state)
        {
            tileEntityMap = new NativeHashMap<int2, Entity>(1024, Allocator.Persistent);

            // Only run when TileData has actually been written to (e.g. IsOccupied changed).
            changedTilesQuery = SystemAPI.QueryBuilder().WithAll<TileData>().Build();
            changedTilesQuery.AddChangedVersionFilter(ComponentType.ReadOnly<TileData>());
            state.RequireForUpdate(changedTilesQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Rebuild map of disabled tiles keyed by board position
            tileEntityMap.Clear();
            foreach (var (tileData, entity) in SystemAPI.Query<RefRO<TileData>>().WithAll<Disabled>().WithEntityAccess())
                tileEntityMap[tileData.ValueRO.PositionIndex] = entity;

            // For every occupied tile, enable any disabled neighbours that are not yet enabled
            foreach (var tileData in SystemAPI.Query<RefRO<TileData>>())
            {
                if (!tileData.ValueRO.IsOccupied) continue;

                int2 pos = tileData.ValueRO.PositionIndex;
                GetAdjacentPositions(in pos, Allocator.Temp, out NativeArray<int2> adjacentPositions);

                foreach (var adjPos in adjacentPositions)
                {
                    if (!tileEntityMap.TryGetValue(adjPos, out Entity adjacentEntity)) continue;

                    // SystemAPI.GetComponent is safe to call on disabled entities
                    var adjacentTileData = SystemAPI.GetComponent<TileData>(adjacentEntity);
                    if (adjacentTileData.IsOccupied || adjacentTileData.IsEnabled) continue;

                    adjacentTileData.IsEnabled = true;
                    ecb.SetComponent(adjacentEntity, adjacentTileData);
                    ecb.RemoveComponent<Disabled>(adjacentEntity);
                }

                adjacentPositions.Dispose();
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (tileEntityMap.IsCreated)
                tileEntityMap.Dispose();
        }

        // Odd-r offset-layout neighbours (parity-aware) — see BoardHelper.GetHexNeighbor.
        // NativeArray is built element-by-element — managed array literals (new int2[]{...})
        // are not allowed in Burst-compiled code.
        [BurstCompile]
        private static void GetAdjacentPositions(in int2 p, Allocator allocator, out NativeArray<int2> result)
        {
            result = new NativeArray<int2>(BoardHelper.HexNeighborCount, allocator, NativeArrayOptions.UninitializedMemory);
            for (int d = 0; d < BoardHelper.HexNeighborCount; d++)
                result[d] = BoardHelper.GetHexNeighbor(p, d);
        }
    }
}
