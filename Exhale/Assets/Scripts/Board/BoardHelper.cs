using UnityEngine;

namespace Exhale.Scripts.Board
{
    public static class BoardHelper
    {
        public static Vector2 GetBoardCenter(int width, int height) {
            return new Vector2(width / 2, height / 2);
        }
    }
}