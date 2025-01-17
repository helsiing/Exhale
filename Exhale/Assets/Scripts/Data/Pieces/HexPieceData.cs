using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    public struct HexPieceComponent : IComponentData
    {
        public int2 PositionIndex; // The tile's position on the grid
        public Entity PieceEntity;   // The associated tile type entity
    }
    
    public class HexPieceData
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        private HexPieceTemplate pieceTemplate;
        public HexPieceTemplate PieceTemplate => pieceTemplate;
        
        public HexPieceData(int x, int y, HexPieceTemplate pieceTemplate)
        {
            this.positionIndex = new Vector2(x, y);
            this.pieceTemplate = pieceTemplate;
        }
    }
}