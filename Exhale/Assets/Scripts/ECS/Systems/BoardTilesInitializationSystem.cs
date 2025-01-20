using Exhale.ECS.Components;
using Exhale.Scripts.Data;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using SphereCollider = Unity.Physics.SphereCollider;

namespace Exhale.ECS.Systems
{
    public partial class BoardTilesInitializationSystem : SystemBase
    {
        private BlobAssetReference<Collider> sphereCollider;
        
        protected override void OnCreate()
        {
            // Run this system once on startup
            RequireForUpdate<BoardData>();
            Debug.Log($"Starting {nameof(BoardTilesInitializationSystem)}...");
            
            sphereCollider = SphereCollider.Create(new SphereGeometry
            {
                Center = float3.zero,
                Radius = 1f // Adjust to match your tile size
            }, CollisionFilter.Default);
            
            base.OnCreate();
        }

        protected override void OnStartRunning()
        {
            EndSimulationEntityCommandBufferSystem ecbSystem =
                World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

            CreateGridJob job = new()
            {
                Ecb = ecb,
                SphereCollider = sphereCollider
            };

            Dependency = job.ScheduleParallel(Dependency);
            ecbSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            // Dispose of the collider when the system is destroyed
            if (sphereCollider.IsCreated)
            {
                sphereCollider.Dispose();
            }
        }
    }
    
    [BurstCompile]
    public partial struct CreateGridJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public BlobAssetReference<Collider> SphereCollider; // Pass the pre-created collider
        
        // Method for processing each BoardData entity
        [UsedImplicitly]
        public void Execute(Entity entity, [EntityIndexInQuery] int entityIndexInQuery, ref BoardData board)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    // Instantiate the prefab
                    if (board.HexTilePrefab != Entity.Null)
                    {
                        Entity hexTileEntity = Ecb.Instantiate(entityIndexInQuery, board.HexTilePrefab);
                        
                        // Add the hex tile tag
                        Ecb.AddComponent(entityIndexInQuery, hexTileEntity, new HexTileTag());
                        
                        // Add a translation for the tile's position in world space
                        float3 worldPos = HexToWorldPositionStatic(x, y);
                        Ecb.SetComponent(entityIndexInQuery, hexTileEntity, LocalTransform.FromPosition(worldPos));
                        
                        // Add PhysicsCollider using the pre-created BlobAssetReference
                        Ecb.AddComponent(entityIndexInQuery, hexTileEntity, new PhysicsCollider
                        {
                            Value = SphereCollider // Use the pre-created collider
                        });
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