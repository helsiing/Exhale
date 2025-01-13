using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class HexTileData : IBoardPositionProvider
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        public HexTileData(Vector2 positionIndex)
        {
            this.positionIndex = positionIndex;
        }
    }
    
    public class HexPieceData : IBoardPositionProvider
    {
        private Vector2 positionIndex;
        public Vector2 PositionIndex => positionIndex;
        
        private HexPieceTemplate pieceTemplate;
        public HexPieceTemplate PieceTemplate => pieceTemplate;
        
        public HexPieceData(Vector2 positionIndex, HexPieceTemplate pieceTemplate)
        {
            this.positionIndex = positionIndex;
            this.pieceTemplate = pieceTemplate;
        }
    }
    
    [RequireComponent(typeof(HexPieceSimulation))]
    [RequireComponent(typeof(HexPiecePresentation))]
    public class HexPiece : MonoBehaviour
    {
        private HexPieceSimulation hexPieceSimulation;
        private HexPiecePresentation hexPiecePresentation;
        private HexPieceData hexPieceData;
        
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