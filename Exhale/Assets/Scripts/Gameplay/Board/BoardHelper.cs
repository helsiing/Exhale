using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public static class BoardHelper
    {
        public static Vector2 GetBoardCenter(int width, int height) 
        {
            return new Vector2(width / 2, height / 2);
        }
        
        public static Vector3 FromCoordinatesToWorldPosition(Vector2 position)
        {
            int row = (int)position.x;
            int col = (int)position.y;
            float xOffset = (col % 2 == 1) ? 0.5f : 0f;
            float zOffSet = 0.87f;
            return new Vector3(row + xOffset, 0, col * zOffSet);
        }
        
        public static Vector3 GetBoardCenterWorldPosition(int rows, int columns, float hexWidth, float hexHeight)
        {
            // Calculate the center position
            float totalWidth = (columns - 1) * hexWidth * 0.75f + hexWidth;
            float totalHeight = (rows - 1) * hexHeight + hexHeight;
            Vector3 centerOffset = new Vector3(totalWidth * 0.5f - hexWidth * 0.5f, 0, totalHeight * 0.5f - hexHeight * 0.5f);

            return -centerOffset;
            // Move the board to center it at (0, 0, 0)
            //board.transform.position = -centerOffset;
        }
    }
}