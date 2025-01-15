using UnityEngine;

namespace Exhale.Scripts.Data
{
    public class HexTileData
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        private bool isEnabled;
        public bool IsEnabled => isEnabled;
        
        private bool hasPiece;
        public bool HasPiece => hasPiece;
        
        public HexTileData(int x, int y, bool hasPiece = false, bool isEnabled = false)
        {
            this.positionIndex = new Vector2(x, y);
            this.hasPiece = hasPiece;
            this.isEnabled = isEnabled;
        }
        
        public void SetEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }
        
        public void SetHasPiece(bool hasPiece)
        {
            this.hasPiece = hasPiece;
        }
    }
}