using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface IHexTileBoardPositionProvider
    {
        public Vector2 BoardPosition { get; }
    }
    
    [RequireComponent(typeof(HexTileSimulation))]
    [RequireComponent(typeof(HexTilePresentation))]
    public class HexTile : MonoBehaviour, IHexTileBoardPositionProvider
    {
        private HexTileSimulation hexTileSimulation;
        private HexTilePresentation hexTilePresentation;
        private HexTileData hexTileData;
        
        public Vector2 BoardPosition => hexTileData.Position;

        private void Awake()
        {
            TryGetComponent(out hexTileSimulation);
            TryGetComponent(out hexTilePresentation);
        }
        
        public void Init(HexTileData hexTileData)
        {
            this.hexTileData = hexTileData;
            hexTileSimulation.Init(OnTileAction);
        }
        
        void OnTileAction()
        {
            Debug.Log($"Tile action: {hexTileData.Position}");
        }
    }
}