using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Components
{
    [Serializable] 
    public struct BoardPosition : IComponentData
    {
        public int2 PositionIndex;
    }
}