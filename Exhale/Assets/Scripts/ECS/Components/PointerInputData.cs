using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.ECS.Components
{
    public struct PointerInputData : IComponentData
    {
        public float3 RayOrigin;
        public float3 RayDirection;
        public bool IsClickDown;
        public bool IsValid;
    }
}
