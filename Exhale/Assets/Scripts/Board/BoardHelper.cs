using UnityEngine;

namespace Exhale.Scripts.Board
{
    public static class BoardHelper
    {
        public static Vector2 GetBoardCenter(int width, int height) 
        {
            return new Vector2(width / 2, height / 2);
        }
        
        public static Vector3 FromCoordinatesToWorldPosition(Vector2 position)
        {
            return FromCoordinatesToWorldPosition((int)position.x, (int)position.y);
        }
        
        public static Vector3 FromCoordinatesToWorldPosition(int x, int y)
        {
            float xOffset = (y % 2 == 1) ? 0.5f : 0f;
            return new Vector3(x + xOffset, 0, y * 0.87f);
        }
    }
}