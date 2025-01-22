using UnityEngine;

namespace Exhale.Scripts.Data
{
    public class HexTileData
    {
        public HexTileData(int x, int y, bool hasPiece = false, bool isEnabled = false)
        {
            PositionIndex = new Vector2(x, y);
            HasPiece = hasPiece;
            IsEnabled = isEnabled;
        }

        public Vector2 PositionIndex { get; }

        public bool IsEnabled { get; private set; }

        public bool HasPiece { get; private set; }

        public void SetEnabled(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public void SetHasPiece(bool hasPiece)
        {
            HasPiece = hasPiece;
        }
    }
}