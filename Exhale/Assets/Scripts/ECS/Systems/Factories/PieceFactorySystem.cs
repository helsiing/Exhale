using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Exhale.ECS.Systems
{
    public partial class PieceFactorySystem : SystemBase
    {
        private NativeList<PieceEntityData> pieceEntities;

        protected override void OnCreate()
        {
            RequireForUpdate<SpawnPiecesConfig>();
            pieceEntities = new NativeList<PieceEntityData>(32, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            // Populate piece catalog once from the baked buffer
            if (pieceEntities.IsEmpty)
            {
                foreach (var buffer in SystemAPI.Query<DynamicBuffer<PieceEntityData>>())
                {
                    for (int i = 0; i < buffer.Length; i++)
                        pieceEntities.Add(buffer[i]);
                    break;
                }
            }

            if (pieceEntities.IsEmpty)
            {
                Debug.LogWarning("[PieceFactorySystem] pieceEntities catalog is empty — SpawnPiecesConfig or PieceEntityData buffer may be missing from the subscene.");
                return;
            }

            // Collect all pending requests before making any structural changes.
            // EntityManager.Instantiate/AddComponentData inside the loop would cause
            // "structural changes during iteration" exceptions.
            var pendingPieceIds = new NativeList<int>(Allocator.Temp);
            var pendingPositions = new NativeList<int2>(Allocator.Temp);
            var pendingEntities  = new NativeList<Entity>(Allocator.Temp);

            foreach (var (request, entity) in SystemAPI.Query<RefRO<PieceCreationRequest>>().WithEntityAccess())
            {
                int pieceId = request.ValueRO.PieceId == -1
                    ? pieceEntities[pieceEntities.Length > 1 ? Random.Range(0, pieceEntities.Length) : 0].PieceId
                    : request.ValueRO.PieceId;

                pendingPieceIds.Add(pieceId);
                pendingPositions.Add(request.ValueRO.PositionIndex);
                pendingEntities.Add(entity);
            }

            // Query is done — structural changes are safe now.
            for (int i = 0; i < pendingPieceIds.Length; i++)
            {
                CreatePiece(pendingPieceIds[i], pendingPositions[i]);
                EntityManager.DestroyEntity(pendingEntities[i]);
            }

            pendingPieceIds.Dispose();
            pendingPositions.Dispose();
            pendingEntities.Dispose();
        }

        protected override void OnDestroy()
        {
            if (pieceEntities.IsCreated)
                pieceEntities.Dispose();
        }

        public void CreatePiece(int pieceId, int2 positionIndex)
        {
            Debug.Log($"[PieceFactorySystem] CreatePiece id={pieceId} pos={positionIndex}. Catalog has {pieceEntities.Length} entries: [{string.Join(", ", System.Linq.Enumerable.Select(pieceEntities.AsArray().ToArray(), e => e.PieceId))}]");
            for (int i = 0; i < pieceEntities.Length; i++)
            {
                if (pieceEntities[i].PieceId != pieceId) continue;

                Entity pieceEntity = EntityManager.Instantiate(pieceEntities[i].PrefabEntity);
                EntityManager.AddComponentData(pieceEntity, new BoardPosition { PositionIndex = positionIndex });
                EntityManager.AddComponentData(pieceEntity, LocalTransform.FromPosition(BoardHelper.HexToWorldPosition(positionIndex)));
                Debug.Log($"[PieceFactorySystem] Spawned piece entity at {positionIndex}");
                return;
            }
            Debug.LogWarning($"[PieceFactorySystem] No piece found for pieceId={pieceId}");
        }
    }
}
