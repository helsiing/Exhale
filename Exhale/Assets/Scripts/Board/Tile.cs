using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class Tile
    {
        private TileType type;
        public TileType Type => type;
        
        private int level;
        public int Level => level;
        
        private Vector2 position;
        public Vector2 Position => position;
        
        public Tile(Vector2 position, TileType type, int level = 0)
        {
            this.position = position;
            this.type = type;
            this.level = level;
        }
    }
}