using Unity.Mathematics;

namespace Exhale.Utils
{
    public static class BoardHelper
    {
        public static float3 HexToWorldPosition(int2 positionIndex)
        {
            return HexToWorldPosition(positionIndex.x, positionIndex.y);
        }
        
        public static float3 HexToWorldPosition(int x, int y)
        {
            var hexWidth = 1.0f;
            var hexHeight = math.sqrt(3) / 2 * hexWidth;
            var xOffset = y % 2 == 0 ? 0 : hexWidth / 2;
            return new float3(x * hexWidth + xOffset, 0, y * hexHeight);
        }
        
        public static int2 GetBoardCenter(int width, int height)
        {
            return new int2(width / 2, height / 2);
        }
    }
}