using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Components
{
    public struct BoardPiece : IComponentData
    {
        public int PieceId;
    }
    
    public struct BoardPosition : IComponentData
    {
        public int2 PositionIndex;
    }
}