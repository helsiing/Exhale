using UnityEngine;

namespace Exhale.Scripts.Data
{
    public class HexTileData
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        private bool isOccupied;
        public bool IsOccupied => isOccupied;
        
        public HexTileData(int x, int y, bool isOccupied)
        {
            this.positionIndex = new Vector2(x, y);
            this.isOccupied = this.isOccupied;
        }
        
        public HexTileData(Vector2 positionIndex, bool isOccupied)
        {
            this.positionIndex = positionIndex;
            this.isOccupied = isOccupied;
        }
    }
}