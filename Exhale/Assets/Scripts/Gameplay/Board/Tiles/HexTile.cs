using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    [RequireComponent(typeof(HexTileSimulation))]
    [RequireComponent(typeof(HexTilePresentation))]
    public class HexTile : MonoBehaviour, IBoardPositionProvider
    {
        private HexTileData hexTileData;
        public Vector2 PositionIndex => hexTileData.PositionIndex;
        
        public void Init(HexTileData hexTileData)
        {
            this.hexTileData = hexTileData;
        }

        
    }
}