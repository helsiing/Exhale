using System;
using Exhale.Scripts.Data;
using Unity.Entities;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public struct PieceEntityData : IBufferElementData, IEquatable<PieceEntityData>
    {
        public int PieceId;
        public Entity PrefabEntity;

        public bool Equals(PieceEntityData other)
        {
            return PieceId == other.PieceId && PrefabEntity.Equals(other.PrefabEntity);
        }

        public override bool Equals(object obj)
        {
            return obj is PieceEntityData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PieceId, PrefabEntity);
        }
    }
    
    public struct SpawnPiecesConfig : IComponentData
    {
    }
    
    public class SpawnPiecesConfigAuthoring : MonoBehaviour
    {
        [SerializeField] private HexPieceTemplateCollection pieceTemplateCollection;
        private class Baker : Baker<SpawnPiecesConfigAuthoring>
        {
            public override void Bake(SpawnPiecesConfigAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                
                DynamicBuffer<PieceEntityData> buffer = AddBuffer<PieceEntityData>(entity);
                foreach (HexPieceTemplate pieceTemplate in authoring.pieceTemplateCollection)
                {
                    if (!pieceTemplate.TryGetTrait(out BoardObject boardObject))
                    {
                        Debug.Log($"Piece {pieceTemplate.name} does not have a BoardObject trait");;
                        return;
                    }
                    
                    if (boardObject.Prefab.TryGetComponent(out PieceAuthoring pieceAuthoring))
                    {
                        buffer.Add(new PieceEntityData
                        {
                            PieceId = pieceAuthoring.PieceTemplate.GetId(),
                            PrefabEntity = GetEntity(boardObject.Prefab, TransformUsageFlags.Dynamic)
                        });
                    }
                }
                
                AddComponent(entity, new SpawnPiecesConfig());
            }
        }
    }
}