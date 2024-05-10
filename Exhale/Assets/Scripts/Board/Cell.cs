using UnityEngine;

namespace Exhale.Scripts.Board
{
    
    public class Cell
    {
        private Vector2 position;

        public Cell(int x, int y)
        {
            position = new Vector2(x, y);
        }
    }
}