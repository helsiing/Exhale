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

    // Subset of the catalog: piece ids whose template has a Ground trait. Used by
    // PrePlaceGroundSystem to pick a random ground type per scattered tile.
    public struct GroundPieceId : IBufferElementData
    {
        public int Value;
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
                DynamicBuffer<GroundPieceId> groundBuffer = AddBuffer<GroundPieceId>(entity);
                foreach (var pieceTemplate in authoring.pieceTemplateCollection)
                {
                    var prefab = pieceTemplate.GetPrefab();
                    if (prefab == null) continue;

                    buffer.Add(new PieceEntityData
                    {
                        PieceId      = pieceTemplate.GetId(),
                        PrefabEntity = GetEntity(prefab, TransformUsageFlags.Dynamic)
                    });

                    if (pieceTemplate.HasTrait<Ground>())
                        groundBuffer.Add(new GroundPieceId { Value = pieceTemplate.GetId() });
                }

                AddComponent(entity, new SpawnPiecesConfig());
            }
        }
    }
}