using System;
using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public struct TileData : IComponentData
    {
        public int2 PositionIndex; // Position in hexagonal grid (use axial or offset coordinates)
        public bool IsOccupied;
        public bool IsEnabled;
        public int  OccupyingPieceId; // 0 = none; set to template.GetId() when a piece lands here
    }
    
    public struct TileDataHighlight : IComponentData
    {
        public bool IsHighlighted;
        public bool IsArmed;
        public bool IsAdjacencyPreview;
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

                // Number of random ground tiles PrePlaceGroundSystem scatters at boot.
                componentData.PrePlacedGroundCount = authoring.boardConfig.PrePlacedGroundCount;
                
                switch (authoring.boardConfig.StartType)
                {
                    case BoardStartType.Random:
                        componentData.StartPosition = new int2(UnityEngine.Random.Range(0, componentData.Width),
                            UnityEngine.Random.Range(0, componentData.Height));
                        break;
                    case BoardStartType.Center:
                        componentData.StartPosition = new int2(componentData.Width / 2, componentData.Height / 2);
                        break;
                    /*case BoardStartType.AtPosition:
                        componentData.StartPosition = componentData.StartPosition;
                        break;*/
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AddComponent(entity, componentData);
            }
        }
    }
}