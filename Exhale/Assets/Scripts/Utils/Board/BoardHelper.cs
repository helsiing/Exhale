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

        // Number of hex neighbours. Pair with GetHexNeighbor(p, 0..5).
        public const int HexNeighborCount = 6;

        // Neighbour of tile `p` in one of the 6 directions, for the odd-r offset layout used
        // by HexToWorldPosition (odd rows are shifted +x/2). The two diagonal columns depend
        // on row parity — using fixed axial offsets puts one neighbour in the wrong cell.
        // Burst-friendly: pure int2 math, no allocations or managed types.
        public static int2 GetHexNeighbor(int2 p, int direction)
        {
            bool odd = (p.y & 1) == 1;
            switch (direction)
            {
                case 0: return new int2(p.x + 1, p.y);                  // E
                case 1: return new int2(p.x - 1, p.y);                  // W
                case 2: return new int2(p.x + (odd ? 1 : 0), p.y - 1);  // NE
                case 3: return new int2(p.x + (odd ? 0 : -1), p.y - 1); // NW
                case 4: return new int2(p.x + (odd ? 1 : 0), p.y + 1);  // SE
                case 5: return new int2(p.x + (odd ? 0 : -1), p.y + 1); // SW
                default: return p;
            }
        }
    }
}