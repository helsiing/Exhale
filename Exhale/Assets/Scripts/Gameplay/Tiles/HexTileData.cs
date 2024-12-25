using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexTileData
    {
        private HexTileType type;
        public HexTileType Type => type;
        
        private int level;
        public int Level => level;
        
        private Vector2 position;
        public Vector2 Position => position;
        
        public HexTileData(Vector2 position, HexTileType type, int level = 0)
        {
            this.position = position;
            this.type = type;
            this.level = level;
        }
    }
}