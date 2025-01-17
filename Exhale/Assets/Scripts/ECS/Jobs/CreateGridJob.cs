using Exhale.Scripts.Data;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Exhale.ECS.Jobs
{
    [BurstCompile]
    public partial struct CreateGridJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;

        // Method for processing each BoardData entity
        [UsedImplicitly]
        public void Execute(Entity entity, [EntityIndexInQuery] int entityIndexInQuery, ref BoardData board)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    // Create a new entity using the command buffer
                    Entity hexTileEntity = Ecb.CreateEntity(entityIndexInQuery);

                    // Add HexPieceComponent data
                    Ecb.AddComponent(entityIndexInQuery, hexTileEntity, new HexPieceComponent
                    {
                        PositionIndex = new int2(x, y)
                    });

                    // Add a translation for the tile's position in world space
                    float3 worldPos = HexToWorldPositionStatic(x, y);
                    Ecb.AddComponent(entityIndexInQuery, hexTileEntity, LocalTransform.FromPosition(worldPos));

                    // Instantiate the prefab
                    if (board.HexTilePrefab != Entity.Null)
                    {
                        Entity instance = Ecb.Instantiate(entityIndexInQuery, board.HexTilePrefab);
                        Ecb.SetComponent(entityIndexInQuery, instance, LocalTransform.FromPosition(worldPos));
                    }
                }
            }
        }

        // Static method for position conversion
        private static float3 HexToWorldPositionStatic(int x, int y)
        {
            float hexWidth = 1.0f;
            float hexHeight = math.sqrt(3) / 2 * hexWidth;
            float xOffset = y % 2 == 0 ? 0 : hexWidth / 2;
            return new float3(x * hexWidth + xOffset, 0, y * hexHeight);
        }
    }
}