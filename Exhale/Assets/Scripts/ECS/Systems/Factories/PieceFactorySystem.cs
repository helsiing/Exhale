using Exhale.Scripts.Data;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Exhale.ECS.Systems
{
    public partial class PieceFactorySystem : SystemBase
    {
        private EntityManager entityManager;
        private NativeHashMap<int, Entity> hashPrefabEntities; // Cache ECS prefab entities

        protected override void OnCreate()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            hashPrefabEntities = new NativeHashMap<int, Entity>(HexPieceTemplateCollection.Values.Count, Allocator.Persistent);
        }
        

        /// <summary>
        ///     Dynamically create a piece at runtime.
        /// </summary>
        public void CreatePiece(int pieceId, int2 positionIndex)
        {
            if (!hashPrefabEntities.TryGetValue(pieceId, out Entity cachedEntity))
            {
                Debug.LogError($"Piece already exists: {pieceId}");
            }

            if (entityManager.HasComponent<Prefab>(cachedEntity))
            {
                Debug.Log("Prefab is valid and has the Prefab tag.");
            }
            else
            {
                Debug.LogError("Entity is not a valid prefab. Ensure it was baked correctly.");
            }
            
            var ecbSystem =
                World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

            CreatePieceJob job = new()
            {
                Ecb = ecb,
                CachedPieceEntity = cachedEntity,
                PositionIndex = positionIndex
            };

            Dependency = job.ScheduleParallel(Dependency);
            ecbSystem.AddJobHandleForProducer(Dependency);
            
        }

        public Entity? CreateRandomPiece(int2 positionIndex)
        {
            var randomPieceIndex = Random.Range(0, hashPrefabEntities.Count);
            var currentIndex = 0;
            foreach (var hashPrefabEntity in hashPrefabEntities)
            {
                if (currentIndex == randomPieceIndex)
                {
                    CreatePiece(hashPrefabEntity.Key, positionIndex);
                }
                currentIndex++;
            }

            return null;
        }

        protected override void OnUpdate()
        {
            EntityQuery prefabQuery = entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(Prefab), typeof(PieceTemplateData) },
                Options = EntityQueryOptions.IncludePrefab 
            });
            
            // Cache the prefab entities
            using var prefabEntities = prefabQuery.ToEntityArray(Allocator.Temp);
            foreach (var prefabEntity in prefabEntities)
            {
                // Get the PieceData component to use as a key
                PieceTemplateData pieceTemplateData = entityManager.GetComponentData<PieceTemplateData>(prefabEntity);
                
                if(hashPrefabEntities.TryGetValue(pieceTemplateData.PieceId, out Entity _)) 
                    continue;
                
                hashPrefabEntities.TryAdd(pieceTemplateData.PieceId, prefabEntity);
                Debug.Log($"Cached {prefabEntity} piece entities for runtime instantiation.");
            }
        }
    }

    [BurstCompile]
    public partial struct CreatePieceJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public Entity CachedPieceEntity;
        [ReadOnly] public int2 PositionIndex;

        [UsedImplicitly]
        public void Execute(Entity entity, [EntityIndexInQuery] int entityIndexInQuery)
        {
            if (CachedPieceEntity != Entity.Null)
            {
                var pieceEntity = Ecb.Instantiate(entityIndexInQuery, CachedPieceEntity);
                Ecb.AddComponent(entityIndexInQuery, pieceEntity, new BoardPosition()
                {
                    PositionIndex = PositionIndex
                });
                Debug.Log("AQUI");
            }
        }
    }
}