using Unity.Entities;

namespace Exhale.ECS.Components
{
    // Singleton tag present while PlacementState == CardLaunching.
    // TileArmingSystem checks for this component to ignore clicks during the card arc.
    public struct PlacementInputBlocked : IComponentData { }
}
