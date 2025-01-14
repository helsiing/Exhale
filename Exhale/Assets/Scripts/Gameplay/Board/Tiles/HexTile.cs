using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface IHexTile
    {
        public void Init(HexTileData hexTileData);
    }
    
    [RequireComponent(typeof(HexTileSimulation))]
    [RequireComponent(typeof(HexTilePresentation))]
    public class HexTile : MonoBehaviour, IHexTile, IBoardPositionProvider
    {
        private HexTileData hexTileData;
        public Vector2 PositionIndex => hexTileData.PositionIndex;
        
        public void Init(HexTileData hexTileData)
        {
            this.hexTileData = hexTileData;
        }

    }
}