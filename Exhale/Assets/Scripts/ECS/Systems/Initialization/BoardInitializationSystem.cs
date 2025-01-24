using Exhale.ECS.Authoring;
using Exhale.Scripts.Data;
using Exhale.Utils;
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
    public partial class BoardInitializationSystem : SystemBase
    {
        private BlobAssetReference<Collider> sphereCollider;
        private PieceFactorySystem pieceFactorySystem;

        protected override void OnCreate()
        {
            RequireForUpdate<BoardDataComponent>();
            Debug.Log($"Starting {nameof(BoardInitializationSystem)}...");

            sphereCollider = SphereCollider.Create(new SphereGeometry
            {
                Center = float3.zero,
                Radius = 1f // Adjust to match your tile size
            }, CollisionFilter.Default);
            
            // Access the PieceFactorySystem
            pieceFactorySystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PieceFactorySystem>();

            base.OnCreate();
        }

        protected override void OnStartRunning()
        {
            var ecbSystem =
                World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

            CreateBoardJob job = new()
            {
                Ecb = ecb,
                SphereCollider = sphereCollider,
            };

            Dependency = job.ScheduleParallel(Dependency);
            ecbSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnUpdate()
        {
            /*BoardDataComponent boardDataComponent = SystemAPI.GetSingleton<BoardDataComponent>();
            for(int i = 0; i < 1; i ++)
            {
                pieceFactorySystem.CreateRandomPiece(new int2(UnityEngine.Random.Range(0, boardDataComponent.Width), UnityEngine.Random.Range(0, boardDataComponent.Height)));
            }*/
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
    public partial struct CreateBoardJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public BlobAssetReference<Collider> SphereCollider;

        [UsedImplicitly]
        public void Execute(Entity entity, [EntityIndexInQuery] int entityIndexInQuery, ref BoardDataComponent board)
        {
            
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    if (board.EmptyTTilePrefabEntity == Entity.Null)
                    {
                        continue;
                    }

                    Entity hexTileEntity = Ecb.Instantiate(entityIndexInQuery, board.EmptyTTilePrefabEntity);
                    Ecb.AddComponent(entityIndexInQuery, hexTileEntity, 
                        new PhysicsCollider { Value = SphereCollider });
                    Ecb.AddComponent(entityIndexInQuery, hexTileEntity, 
                        new TileData
                        {
                            PositionIndex = new int2(x, y),
                            IsOccupied = false
                        });

                    Ecb.SetComponent(entityIndexInQuery, hexTileEntity,
                        LocalTransform.FromPosition(BoardHelper.HexToWorldPosition(x, y)));
                }
            }
        }
    }
}