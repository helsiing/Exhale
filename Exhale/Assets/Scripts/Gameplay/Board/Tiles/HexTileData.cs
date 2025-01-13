using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexTileData
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        public HexTileData(Vector2 positionIndex)
        {
            this.positionIndex = positionIndex;
        }
    }
}