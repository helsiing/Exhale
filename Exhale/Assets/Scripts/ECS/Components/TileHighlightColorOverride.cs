using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Exhale.ECS.Components
{
    // Per-entity _BaseColor override consumed by Entities Graphics.
    // Adding this component to a tile entity tints it without modifying the shared material.
    // Removing it restores the material's original colour.
    [MaterialProperty("_BaseColor")]
    public struct TileHighlightColorOverride : IComponentData
    {
        public float4 Value;
    }
}
