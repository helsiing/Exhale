using Exhale.ECS.Authoring;
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

        public void OnCreate(ref SystemState state)
        {
            tileEntityMap = new NativeHashMap<int2, Entity>(1024, Allocator.Persistent); // Adjust capacity as needed
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Clear the map and rebuild it each frame
            tileEntityMap.Clear();

            // Populate the HashMap with tile positions and their corresponding entities
            foreach (var (tileData, entity) in SystemAPI.Query<RefRO<TileData>>().WithAll<Disabled>().WithEntityAccess())
            {
                tileEntityMap[tileData.ValueRO.PositionIndex] = entity;
            }

            // Query all occupied tiles (pieces placed)
            foreach (var (tileData, entity) in SystemAPI.Query<RefRW<TileData>>().WithEntityAccess())
            {
                if (!tileData.ValueRO.IsOccupied) continue; // Ignore empty tiles

                // Get adjacent tile positions
                NativeArray<int2> adjacentPositions =
                    GetAdjacentTilePositions(tileData.ValueRO.PositionIndex, Allocator.Temp);

                // Query adjacent tiles based on position
                foreach (var adjPos in adjacentPositions)
                {
                    if (tileEntityMap.TryGetValue(adjPos, out Entity adjacentEntity)) // Fast lookup in the hash map
                    {
                        var adjacentTileData = state.EntityManager.GetComponentData<TileData>(adjacentEntity);

                        if (!adjacentTileData.IsOccupied && !adjacentTileData.IsEnabled)
                        {
                            // Enable tile
                            adjacentTileData.IsEnabled = true;
                            ecb.SetComponent(adjacentEntity, adjacentTileData);
                            ecb.RemoveComponent<Disabled>(adjacentEntity);
                        }
                    }
                }

                adjacentPositions.Dispose();
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        public void OnDestroy(ref SystemState state)
        {
            if (tileEntityMap.IsCreated)
                tileEntityMap.Dispose();
        }

        private NativeArray<int2> GetAdjacentTilePositions(int2 position, Allocator allocator)
        {
            return new NativeArray<int2>(new int2[]
            {
                new int2(position.x + 1, position.y),
                new int2(position.x - 1, position.y),
                new int2(position.x, position.y + 1),
                new int2(position.x, position.y - 1),
                new int2(position.x + 1, position.y - 1),
                new int2(position.x - 1, position.y + 1)
            }, allocator);
        }
    }
}
