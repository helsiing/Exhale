using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Components
{
    public struct TileHighlightConfigData : IComponentData
    {
        public float4 PreviewColor;
        public float4 ValidPlacementColor;
        public float4 InvalidRequirementsColor;  // building card selected, tile cannot be placed here
        public float4 RequirementSatisfiedColor; // hover detail: required piece IS present
        public float4 RequirementMissingColor;   // hover detail: required piece is MISSING
    }
}
