using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexPieceData
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        private HexPieceTemplate pieceTemplate;
        public HexPieceTemplate PieceTemplate => pieceTemplate;
        
        public HexPieceData(Vector2 positionIndex, HexPieceTemplate pieceTemplate)
        {
            this.positionIndex = positionIndex;
            this.pieceTemplate = pieceTemplate;
        }
    }
}