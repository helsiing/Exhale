using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface IHexPiece
    {
        public void Init(HexPieceData hexPieceData);
        public void Show();
    }
    
    public class HexPiece : MonoBehaviour, IHexPiece, IBoardPositionProvider
    {
        private IHexPieceSimulation pieceSimulation;
        private IHexPiecePresentation piecePresentation;
        private HexPieceData hexPieceData;
        
        public Vector2 PositionIndex => hexPieceData.PositionIndex;
        
        public void Init(HexPieceData hexPieceData)
        {
            this.hexPieceData = hexPieceData;
            pieceSimulation = GetComponent<IHexPieceSimulation>();
            piecePresentation = GetComponent<IHexPiecePresentation>();
        }

        public void Show()
        {
            piecePresentation.Show();
        }
    }
}