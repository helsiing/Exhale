using Unity.Entities;

namespace Exhale.ECS.Components
{
    // Added to the up-to-6 currently-disabled neighbors of the armed tile to show a
    // "would unlock" preview. Cleared together with TileArmedTag on disarm or re-arm.
    public struct TileAdjacencyPreviewTag : IComponentData { }
}
