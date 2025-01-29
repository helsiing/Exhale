using ECS.Scripts.Managers;
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
using MeshCollider = UnityEngine.MeshCollider;
using SphereCollider = Unity.Physics.SphereCollider;

namespace Exhale.ECS.Systems
{
    public partial class BoardInitializationSystem : SystemBase
    {
        private BlobAssetReference<Collider> sphereCollider;
        private PieceFactorySystem pieceFactorySystem;
        private Entity boardInitializedEventEntity;
        
        protected override void OnCreate()
        {
            RequireForUpdate<BoardDataComponent>();
            Debug.Log($"Starting {nameof(BoardInitializationSystem)}...");
            
            sphereCollider = SphereCollider.Create(new SphereGeometry
            {
                Center = float3.zero,
                Radius = .25f // Adjust to match your tile size
            }, CollisionFilter.Default);
            
            
            // Access the PieceFactorySystem
            pieceFactorySystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PieceFactorySystem>();
            
            boardInitializedEventEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(boardInitializedEventEntity, new BoardInitializedEvent { IsInitialized = false });

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
                BoardInitializedEventEntity = boardInitializedEventEntity
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
            
            BoardInitializedEvent boardEvent = SystemAPI.GetComponent<BoardInitializedEvent>(boardInitializedEventEntity);

            if (!boardEvent.IsInitialized) return; // Skip if the board isn't ready
            
            BoardEventManager.TriggerBoardInitialized(boardEvent.StartPosition);
            EntityManager.SetComponentData(boardInitializedEventEntity, new BoardInitializedEvent { IsInitialized = false });

            Debug.Log("✅ Board Initialized - Unity Event Triggered!");
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
        public Entity BoardInitializedEventEntity;

        [UsedImplicitly]
        public void Execute(Entity entity, [EntityIndexInQuery] int entityIndexInQuery, ref BoardDataComponent board)
        {
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    if (board.EmptyTTilePrefabEntity == Entity.Null)
                    {
                        continue;
                    }

                    bool isStartTile = x == board.StartPosition.x && y == board.StartPosition.y;
                    Entity hexTileEntity = Ecb.Instantiate(entityIndexInQuery, board.EmptyTTilePrefabEntity);
                    Ecb.AddComponent(entityIndexInQuery, hexTileEntity, 
                        new PhysicsCollider { Value = SphereCollider });
                    Ecb.AddComponent(entityIndexInQuery, hexTileEntity, 
                        new TileData
                        {
                            PositionIndex = new int2(x, y),
                            IsOccupied = false,
                            IsEnabled = isStartTile
                        });

                    Ecb.SetComponent(entityIndexInQuery, hexTileEntity,
                        LocalTransform.FromPosition(BoardHelper.HexToWorldPosition(x, y)));
                }
            }
            
            Ecb.SetComponent(0, BoardInitializedEventEntity, new BoardInitializedEvent
            {
                IsInitialized = true,
                StartPosition = board.StartPosition
            });
            Debug.Log($"Board initialized with {board.Width}x{board.Height} tiles and start position at {board.StartPosition}");
        }
        
    }
}