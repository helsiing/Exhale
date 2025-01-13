using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexTileData
    {
        private Vector2 position;
        public Vector2 Position => position;
        
        public HexTileData(Vector2 position)
        {
            this.position = position;
        }
    }
}