using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class BoardLogic
    {
        private HexTileData[,] tiles;
        public HexTileData[,] Tiles => tiles;
        
        private HexPieceData[,] pieces;
        public HexPieceData[,] Pieces => pieces;
        
        public void InitBoard(int width, int height)
        {
            tiles = new HexTileData[width, height];
            pieces = new HexPieceData[width, height];
            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < height; col++)
                {
                    HexTileData hexTileData = new HexTileData(new Vector2(row, col));
                    tiles[row, col] = hexTileData;
                }
            }
        }
        
        public HexPieceData PlacePiece(Vector2 position, HexPieceTemplate pieceTemplate = null)
        {
            int row = (int) position.x;
            int col = (int) position.y;

            if (!BoardHelper.IsWithinBounds(pieces, row, col))
            {
                return null;
            }

            // if it's null let's get a random one
            if (pieceTemplate == null)
            {
                pieceTemplate = HexPieceFactory.GetRandomTemplate();
            }
            
            HexPieceData hexPieceData = new HexPieceData(new Vector2(row, col), pieceTemplate);
            pieces[row, col] = hexPieceData;
            return hexPieceData;

        }
    }
}