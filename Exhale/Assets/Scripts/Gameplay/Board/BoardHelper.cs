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
            return new Vector3(row + xOffset, 0, col * 0.87f);
        }
    }
}