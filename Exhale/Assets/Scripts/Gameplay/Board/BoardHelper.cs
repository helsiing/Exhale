using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public static class BoardHelper
    {
        public static bool IsWithinBounds(HexPieceData[,] pieces, int row, int col)
        {
            if (row > pieces.GetLength(0) || col > pieces.GetLength(1))
            {
                return false;
            }

            return true;
        }
        
        public static Vector2 GetBoardCenter(int width, int height) 
        {
            return new Vector2(width / 2, height / 2);
        }
        
        public static Vector3 FromCoordinatesToWorldPosition(Vector2 position, int totalRows, int totalCols)
        {
            int row = (int)position.x;
            int col = (int)position.y;

            // Hexagon offsets (assuming flat-topped hexes)
            float xOffset = (col % 2 == 1) ? 0.5f : 0f;
            float zOffset = 0.87f; // Distance between rows (based on hex height)

            // Calculate raw world position (bottom-left origin)
            Vector3 rawPosition = new Vector3(row + xOffset, 0, col * zOffset);

            // Calculate board center offset
            float boardWidth = (totalRows - 1) + 0.5f;  // Approx width of the board
            float boardHeight = (totalCols - 1) * zOffset; // Approx height of the board
            Vector3 boardCenterOffset = new Vector3(boardWidth / 2f, 0, boardHeight / 2f);

            // Offset the position to center the board at (0, 0, 0)
            return rawPosition - boardCenterOffset;
        }
        
    }
}