using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Components
{
    public struct HexPieceComponent : IComponentData
    {
        public int2 PositionIndex; // The tile's position on the grid
        public Entity PieceEntity;   // The associated tile type entity
    }
}