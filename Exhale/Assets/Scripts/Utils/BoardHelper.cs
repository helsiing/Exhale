using Unity.Mathematics;

namespace Exhale.Utils
{
    public static class BoardHelper
    {
        public static float3 HexToWorldPosition(int x, int y)
        {
            var hexWidth = 1.0f;
            var hexHeight = math.sqrt(3) / 2 * hexWidth;
            var xOffset = y % 2 == 0 ? 0 : hexWidth / 2;
            return new float3(x * hexWidth + xOffset, 0, y * hexHeight);
        }
    }
}