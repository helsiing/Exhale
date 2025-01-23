using System.Linq;
using Exhale.ECS.Authoring;
using Exhale.Scripts.Data;
using Exhale.Utils;
using JetBrains.Annotations;
using Unity.Burst;
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
        private EntityManager entityManager;

        protected override void OnCreate()
        {
            RequireForUpdate<SpawnPiecesConfig>();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
           
            
        }

        public void CreateRandomPiece(int2 positionIndex)
        {
            int pieceId = int.MinValue;
            Entity piecePrefabEntity = Entity.Null;
            
            Entities.ForEach((Entity entity, DynamicBuffer<PieceEntityData> buffer) =>
            {
                int randomIndex = buffer.Length > 1 ? Random.Range(0, buffer.Length) : 0;
                pieceId = buffer[randomIndex].PieceId;
                piecePrefabEntity = buffer[randomIndex].PrefabEntity;
                
            }).WithoutBurst().Run(); // Use WithoutBurst for simplicity during debugging
            
            Entity pieceEntity = entityManager.Instantiate(piecePrefabEntity);
            entityManager.AddComponentData(pieceEntity, new BoardPosition {PositionIndex = positionIndex});
            entityManager.AddComponentData(pieceEntity,
                LocalTransform.FromPosition(BoardHelper.HexToWorldPosition(positionIndex)));

        }
        
    }
}