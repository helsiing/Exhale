using Exhale.Scripts.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Exhale.Gameplay
{
    public interface IPieceTemplateProvider
    {
        public HexPieceTemplate PieceTemplate { get; }
    }
    
    public interface IHexPiece : IPieceTemplateProvider, IBoardPositionProvider
    {
        public void Init(HexPieceData hexPieceData);
        public void Show();
    }
    
    public class HexPiece : MonoBehaviour, IHexPiece
    {
        private IHexPieceSimulation pieceSimulation;
        private IHexPiecePresentation piecePresentation;
        private HexPieceData hexPieceData;
        
        public Vector2 PositionIndex => hexPieceData.PositionIndex;
        public HexPieceTemplate PieceTemplate => hexPieceData.PieceTemplate;

        
        public void Init(HexPieceData hexPieceData)
        {
            this.hexPieceData = hexPieceData;
            pieceSimulation = GetComponent<IHexPieceSimulation>();
            
            piecePresentation = GetComponent<IHexPiecePresentation>();
            piecePresentation.Init(this);
        }

        public void Show()
        {
            piecePresentation.Show();
        }

    }
}