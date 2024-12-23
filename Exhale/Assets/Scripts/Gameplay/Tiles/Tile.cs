using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface ITileBoardPositionProvider
    {
        public Vector2 BoardPosition { get; }
    }
    
    [RequireComponent(typeof(TileSimulation))]
    [RequireComponent(typeof(TilePresentation))]
    public class Tile : MonoBehaviour, ITileBoardPositionProvider
    {
        private TileSimulation tileSimulation;
        private TilePresentation tilePresentation;
        private TileData tileData;
        
        public Vector2 BoardPosition => tileData.Position;

        private void Awake()
        {
            TryGetComponent(out tileSimulation);
            TryGetComponent(out tilePresentation);
        }
        
        public void Init(TileData tileData)
        {
            this.tileData = tileData;
            tileSimulation.Init(OnTileAction);
        }
        
        void OnTileAction()
        {
            Debug.Log($"Tile action: {tileData.Position} - {tileData.Type}");
        }
    }
}