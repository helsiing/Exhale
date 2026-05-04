using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Components
{
    public struct TileHighlightConfigData : IComponentData
    {
        public float4 ArmedColor;
        public float4 PreviewColor;
        public float4 ValidPlacementColor;
    }
}
