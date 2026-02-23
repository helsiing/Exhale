using System.Collections.Generic;
using System.Linq;
using Exhale.ECS.Authoring;
using Exhale.ECS.Components;
using Exhale.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Exhale.ECS.Systems
{
    public partial class PieceFactorySystem : SystemBase
    {
        private EntityManager entityManager;
        private List<PieceEntityData> pieceEntities { get; set; }

        protected override void OnCreate()
        {
            RequireForUpdate<SpawnPiecesConfig>();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            pieceEntities = ListPool<PieceEntityData>.Get();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, DynamicBuffer<PieceEntityData> buffer) =>
            {
                foreach (PieceEntityData pieceEntityData in buffer)
                {
                    if (!pieceEntities.Contains(pieceEntityData))
                    {
                        pieceEntities.Add(pieceEntityData);
                    }
                }
                
            }).WithoutBurst().Run(); // Use WithoutBurst for simplicity during debugging
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ListPool<PieceEntityData>.Release(pieceEntities);
        }

        public void CreateRandomPiece(int2 positionIndex)
        {
            int randomIndex = pieceEntities.Count > 1 ? Random.Range(0, pieceEntities.Count) : 0;
            int randomPieceId = pieceEntities[randomIndex].PieceId;
            
            CreatePiece(randomPieceId, positionIndex);
        }

        public void CreatePiece(int pieceId, int2 positionIndex)
        {
            PieceEntityData pieceEntityData = pieceEntities.FirstOrDefault(x => x.PieceId.Equals(pieceId));
            Entity pieceEntity = entityManager.Instantiate(pieceEntityData.PrefabEntity);
            entityManager.AddComponentData(pieceEntity, new BoardPosition {PositionIndex = positionIndex});
            entityManager.AddComponentData(pieceEntity,
                LocalTransform.FromPosition(BoardHelper.HexToWorldPosition(positionIndex)));
            
        }

    }
}