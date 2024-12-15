using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class BoardLogic
    {
        private Tile[,] tiles;
        public Tile[,] Tiles => tiles;
        
        public void InitBoard(int width, int height)
        {
            tiles = new Tile[width, height];
            
            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < height; col++)
                {
                    Tile tile = new Tile(new Vector2(row, col), 0);
                    tiles[row, col] = tile;
                }
            }
        }

        public bool PlaceTile(int row, int col, TileType tileType)
        {
            if (row <= tiles.GetLength(0) && col <= tiles.GetLength(1))
            {
                if (tiles[row, col].Type == 0)
                {
                    Tile tile = new Tile(new Vector2(row, col), tileType);
                    tiles[row, col] = tile;
                    return true;
                }
            }
            
            return false;
        }
        
        
    }
}