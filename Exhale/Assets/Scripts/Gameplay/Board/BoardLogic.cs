using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class BoardLogic
    {
        private TileData[,] tiles;
        public TileData[,] Tiles => tiles;
        
        public void InitBoard(int width, int height)
        {
            tiles = new TileData[width, height];
            
            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < height; col++)
                {
                    TileData tileData = new TileData(new Vector2(row, col), 0);
                    tiles[row, col] = tileData;
                }
            }
        }
        
        public TileData PlaceTile(Vector2 position, TileType tileType)
        {
            int row = (int) position.x;
            int col = (int) position.y;

            return PlaceTile(row, col, tileType);
        }

        public TileData PlaceTile(int row, int col, TileType tileType)
        {
            if (row <= tiles.GetLength(0) && col <= tiles.GetLength(1))
            {
                if (tiles[row, col].Type == 0)
                {
                    TileData tileData = new TileData(new Vector2(row, col), tileType);
                    tiles[row, col] = tileData;
                    return tileData;
                }
            }
            
            return null;
        }
        
        
    }
}