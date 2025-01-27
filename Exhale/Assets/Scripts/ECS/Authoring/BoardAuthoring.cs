using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public struct TileData : IComponentData
    {
        public int2 PositionIndex; // Position in hexagonal grid (use axial or offset coordinates)
        public bool IsOccupied;   // Whether this tile is occupied by a piece
    }
    
    public struct TileDataHighlight : IComponentData
    {
        public bool IsHighlighted;
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
                componentData.EmptyTTilePrefabEntity = GetEntity(authoring.boardConfig.EmptyTilePrefab,
                    TransformUsageFlags.Dynamic);

                AddComponent(entity, componentData);
            }
        }
    }
}