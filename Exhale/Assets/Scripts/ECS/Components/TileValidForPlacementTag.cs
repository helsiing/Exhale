using Unity.Entities;

namespace Exhale.ECS.Components
{
    // Added to every enabled, unoccupied tile that accepts the currently selected card.
    // Written by ValidTileHighlightSystem when a card is selected; removed on deselect.
    public struct TileValidForPlacementTag : IComponentData { }
}
