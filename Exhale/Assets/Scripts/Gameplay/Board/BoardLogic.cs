using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class BoardLogic
    {
        private HexTileData[,] tilesData;
        public HexTileData[,] TilesData => tilesData;
        
        private HexPieceData[,] piecesData;
        public HexPieceData[,] PiecesData => piecesData;
        
        public void InitBoard(int width, int height)
        {
            tilesData = new HexTileData[width, height];
            piecesData = new HexPieceData[width, height];
            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < height; col++)
                {
                    HexTileData hexTileData = new HexTileData(new Vector2(row, col));
                    tilesData[row, col] = hexTileData;
                }
            }
        }
        
        public HexPieceData PlacePiece(Vector2 position, HexPieceTemplate pieceTemplate = null)
        {
            int row = (int) position.x;
            int col = (int) position.y;

            if (!BoardHelper.IsWithinBounds(piecesData, row, col))
            {
                return null;
            }
            
            HexPieceData hexPieceData = new HexPieceData(new Vector2(row, col), pieceTemplate);
            piecesData[row, col] = hexPieceData;
            return hexPieceData;

        }
    }
}