using Unity.Entities;

namespace Exhale.ECS.Components
{
    // Added to the tile entity that the player has selected as a placement destination.
    // Consumed by TileHighlightSystem (armed glow) and removed by TileArmingSystem on
    // disarm or re-arm.
    public struct TileArmedTag : IComponentData { }
}
