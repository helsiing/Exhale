using System.Numerics;
using Unity.Mathematics;

namespace Exhale.Utils
{
    public static class Utils
    {
        public static Vector3 ToVector(this float3 obj)
        {
            return new Vector3(obj.x, obj.y, obj.z);
        }
    }
}