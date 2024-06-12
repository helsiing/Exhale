using UnityEngine;

namespace Exhale.Scripts.Board
{
    
    public class Cell
    {
        private Vector2 position;
        public Vector2 Position => position;
        
        private GameObject cellObject;
        public GameObject CellObject => cellObject;

        public Cell(int x, int y, GameObject cellObject)
        {
            position = new Vector2(x, y);
            this.cellObject = cellObject;
        }
    }
}