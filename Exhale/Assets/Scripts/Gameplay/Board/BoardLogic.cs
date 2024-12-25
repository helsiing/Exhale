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
                    HexTileData hexTileData = new HexTileData(new Vector2(row, col), 0);
                    tiles[row, col] = hexTileData;
                }
            }
        }
        
        public HexTileData PlaceTile(Vector2 position, HexTileType hexTileType)
        {
            int row = (int) position.x;
            int col = (int) position.y;

            return PlaceTile(row, col, hexTileType);
        }

        public HexTileData PlaceTile(int row, int col, HexTileType hexTileType)
        {
            if (row <= tiles.GetLength(0) && col <= tiles.GetLength(1))
            {
                if (tiles[row, col].Type == 0)
                {
                    HexTileData hexTileData = new HexTileData(new Vector2(row, col), hexTileType);
                    tiles[row, col] = hexTileData;
                    return hexTileData;
                }
            }
            
            return null;
        }
        
        
    }
}