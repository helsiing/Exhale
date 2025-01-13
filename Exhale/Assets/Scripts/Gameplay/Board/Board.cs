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
            
            boardSimulation.OnTileClickedEvent += OnTileClicked;
        }

        private void Start() 
        {
            boardLogic.InitBoard(boardConfig.Width, boardConfig.Height);
            
            Vector2 centerCellBoardPosition = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
            HexPieceTemplate centerPieceTemplate = HexPieceFactory.GetRandomTemplate<Building>();
            PlacePiece(centerCellBoardPosition, centerPieceTemplate, false);
            
            boardPresentation.DrawBoard(boardLogic.Tiles, boardLogic.Pieces);
        }
        
        private void PlacePiece(Vector2 positionIndex, HexPieceTemplate pieceTemplate = null, bool shouldDraw = true)
        {
            HexPieceData hexPieceData = boardLogic.PlacePiece(positionIndex, pieceTemplate);
            Assert.IsNotNull(hexPieceData, "tile != null");

            if (!shouldDraw)
            {
                return;
            }

            boardPresentation.DrawPiece(hexPieceData);
        }
        
        void OnTileClicked(Vector2 position)
        {
            PlacePiece(position, HexPieceFactory.GetRandomTemplate());
        }

        private void OnDestroy()
        {
            boardSimulation.OnTileClickedEvent -= OnTileClicked;
        }
    }
}