using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public struct HexTileData : IComponentData
    {
        public int2 GridPosition; // Position in hexagonal grid (use axial or offset coordinates)
        public bool IsOccupied;   // Whether this tile is occupied by a piece
        public bool IsUnlocked;   // Whether this tile is available for placement
    }
    
    public class BoardAuthoring : MonoBehaviour
    {
        [SerializeField] private BoardConfig boardConfig;

        private class Baker : Baker<BoardAuthoring>
        {
            public override void Bake(BoardAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);

                var componentData = authoring.boardConfig.Data;
                componentData.EmptyPiecePrefabEntity = GetEntity(authoring.boardConfig.EmptyPiecePrefab,
                    TransformUsageFlags.Dynamic);

                AddComponent(entity, componentData);
            }
        }
    }
}