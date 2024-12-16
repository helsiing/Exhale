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

        public void Init(TileData tileData)
        {
            this.tileData = tileData;
        }
        
        private void Awake()
        {
            TryGetComponent(out tileSimulation);
            TryGetComponent(out tilePresentation);
        }

        public void Init()
        {
            
        }
    }
}