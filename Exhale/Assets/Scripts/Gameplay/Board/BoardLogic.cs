using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class BoardLogic
    {
        private HexTileData[,] tiles;
        public HexTileData[,] Tiles => tiles;
        
        public void InitBoard(int width, int height)
        {
            tiles = new HexTileData[width, height];
            
            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < height; col++)
                {
                    HexTileData hexTileData = new HexTileData(new Vector2(row, col));
                    tiles[row, col] = hexTileData;
                }
            }
        }
        
        public HexTileData PlaceTile(Vector2 position)
        {
            int row = (int) position.x;
            int col = (int) position.y;

            if (row > tiles.GetLength(0) || col > tiles.GetLength(1))
            {
                return null;
            }

            HexTileData hexTileData = new HexTileData(new Vector2(row, col));
            tiles[row, col] = hexTileData;
            return hexTileData;

        }
    }
}