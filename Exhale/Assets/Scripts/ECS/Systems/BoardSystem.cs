using System.Collections.Generic;
using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BoardSystem : SystemBase
{
    private List<Entity> hexTileEntities = new List<Entity>();
    protected override void OnCreate()
    {
        // Run this system once on startup
        RequireForUpdate<BoardData>();
    }

    protected override void OnStartRunning()
    {
        // Create a command buffer system to handle structural changes
        var ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // Query all BoardData entities and process them in parallel
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref BoardData board) =>
            {
                for (int y = 0; y < board.Height; y++)
                {
                    for (int x = 0; x < board.Width; x++)
                    {
                        // Create a new entity using the command buffer
                        var hexTileEntity = ecb.CreateEntity(entityInQueryIndex);

                        // Add HexPieceComponent data
                        ecb.AddComponent(entityInQueryIndex, hexTileEntity, new HexPieceComponent
                        {
                            PositionIndex = new int2(x, y)
                        });

                        // Add a translation for the tile's position in world space
                        float3 worldPos = BoardSystem.HexToWorldPosition(x, y);
                        ecb.AddComponent(entityInQueryIndex, hexTileEntity, LocalTransform.FromPosition(worldPos));

                        // Instantiate the prefab
                        if (board.HexTilePrefab != Entity.Null)
                        {
                            var instance = ecb.Instantiate(entityInQueryIndex, board.HexTilePrefab);
                            ecb.SetComponent(entityInQueryIndex, instance, LocalTransform.FromPosition(worldPos));
                        }
                    }
                }

                // Destroy the board entity after initialization (optional)
                ecb.DestroyEntity(entityInQueryIndex, entity);

            }).ScheduleParallel();

        // Ensure the command buffer system runs after the job completes
        ecbSystem.AddJobHandleForProducer(Dependency);
    }

    protected override void OnUpdate()
    {
        
    }

    private static float3 HexToWorldPosition(int x, int y)
    {
        // Example: Flat-topped hex grid world positioning
        float hexWidth = 1.0f; // Customize based on hex size
        float hexHeight = math.sqrt(3) / 2 * hexWidth;
        float xOffset = y % 2 == 0 ? 0 : hexWidth / 2;
        return new float3(x * hexWidth + xOffset, 0, y * hexHeight);
    }

    private void InstantiatePrefab(EntityManager entityManager, Entity prefab, float3 position)
    {
        Entity instance = entityManager.Instantiate(prefab);
        entityManager.SetComponentData(instance, LocalTransform.FromPosition(position));
    }
}

public partial struct SpawnPieceJob : IJobEntity
{
    public void Execute(ref BoardData board)
    {
        
    }
}

