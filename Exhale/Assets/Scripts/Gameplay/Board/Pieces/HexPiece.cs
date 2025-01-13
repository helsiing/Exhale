using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    [RequireComponent(typeof(HexPieceSimulation))]
    [RequireComponent(typeof(HexPiecePresentation))]
    public class HexPiece : MonoBehaviour, IBoardPositionProvider
    {
        private HexPieceSimulation hexPieceSimulation;
        private HexPiecePresentation hexPiecePresentation;
        private HexPieceData hexPieceData;
        public Vector2 PositionIndex => hexPieceData.PositionIndex;
        
        private void Awake()
        {
            TryGetComponent(out hexPieceSimulation);
            TryGetComponent(out hexPiecePresentation);
        }
        
        public void Init(HexPieceData hexPieceData)
        {
            this.hexPieceData = hexPieceData;
            hexPieceSimulation.Init(OnTileAction);
        }
        
        void OnTileAction()
        {
            Debug.Log($"Tile action: {hexPieceData.PositionIndex}");
        }
    }
}