using UnityEngine;

namespace Exhale.Scripts.Data
{
    public class HexTileData
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        private HexPieceTemplate expectedPieceTemplate;
        public HexPieceTemplate ExpectedPieceTemplate => expectedPieceTemplate;
        
        public HexTileData(int x, int y, HexPieceTemplate expectedPieceTemplate = null)
        {
            this.positionIndex = new Vector2(x, y);
            this.expectedPieceTemplate = expectedPieceTemplate;
        }
        
        public HexTileData(Vector2 positionIndex, HexPieceTemplate expectedPieceTemplate = null)
        {
            this.positionIndex = positionIndex;
            this.expectedPieceTemplate = expectedPieceTemplate;
        }
    }
}