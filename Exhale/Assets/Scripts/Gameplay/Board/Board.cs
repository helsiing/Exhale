using Exhale.Scripts.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Scripts.Gameplay
{
    [RequireComponent(typeof(BoardSimulation))]
    [RequireComponent(typeof(BoardPresentation))]
    public class Board : MonoBehaviour
    {
        [SerializeField] private BoardConfig boardConfig;
        
        private BoardSimulation boardSimulation;
        private BoardPresentation boardPresentation;
        private readonly BoardLogic boardLogic = new();

        private void Awake()
        {
            TryGetComponent(out boardSimulation);
            TryGetComponent(out boardPresentation);
            
            boardSimulation.OnPlacePieceEvent += OnPiecePlaced;
        }

        private void Start() 
        {
            boardLogic.InitBoard(boardConfig.Width, boardConfig.Height);
            
            Vector2 centerCellBoardPosition = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
            PlacePiece(centerCellBoardPosition);
            boardPresentation.DrawBoard(boardLogic.Tiles, boardLogic.Pieces);
         
            
        }
        
        private void PlacePiece(Vector2 positionIndex, HexPieceTemplate pieceTemplate = null)
        {
            HexPieceData hexPieceData = boardLogic.PlacePiece(positionIndex, pieceTemplate);
            Assert.IsNotNull(hexPieceData, "tile != null");
            boardPresentation.DrawPiece(hexPieceData);
        }
        
        void OnPiecePlaced(Vector2 position)
        {
            //HexPieceData hexTileData = new HexPieceData(position);
            //PlacePiece(hexTileData);
        }

        private void OnDestroy()
        {
            boardSimulation.OnPlacePieceEvent -= OnPiecePlaced;
        }
    }
}