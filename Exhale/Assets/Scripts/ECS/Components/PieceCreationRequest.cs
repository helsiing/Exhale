using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Components
{
    public struct PieceCreationRequest : IComponentData
    {
        public int2 PositionIndex;
        public int PieceId; // -1 = pick randomly from available pieces
    }
}
