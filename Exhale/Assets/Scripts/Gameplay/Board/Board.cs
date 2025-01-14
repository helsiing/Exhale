using Exhale.Scripts.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Scripts.Gameplay
{
    public interface IBoard
    {
    }

    [RequireComponent(typeof(BoardSimulation))]
    [RequireComponent(typeof(BoardPresentation))]
    public class Board : MonoBehaviour, IBoard
    {
        [SerializeField] private BoardConfig boardConfig;
        
        private IHexTile[,] tiles;
        public IHexTile[,] Tiles => tiles;
        
        private IHexPiece[,] pieces;
        public IHexPiece[,] Pieces => pieces;
        
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
            InitBoard();
            
            Vector2 centerCellBoardPosition = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
            HexPieceTemplate centerPieceTemplate = HexPieceFactory.GetRandomTemplate<Building>();
            PlacePiece(centerCellBoardPosition, centerPieceTemplate);
            
            boardPresentation.ShowBoard(pieces);
        }

        private void InitBoard()
        {
            tiles = new IHexTile[boardConfig.Width, boardConfig.Height];
            pieces = new IHexPiece[boardConfig.Width, boardConfig.Height];
            
            boardLogic.InitBoard(boardConfig.Width, boardConfig.Height);
            boardPresentation.InitBoard(boardLogic.TilesData);
            for (int row = 0; row < boardConfig.Width; row++)
            {
                for (int col = 0; col < boardConfig.Height; col++)
                {
                    var tileGameObject = boardPresentation.SetTileGameObject(boardLogic.TilesData[row, col]);
                    if (tileGameObject != null && tileGameObject.TryGetComponent(out HexTile hexTile))
                    {
                        hexTile.Init(boardLogic.TilesData[row, col]);
                        tiles[row, col] = hexTile;
                    }
                    var pieceGameObject = boardPresentation.SetPieceGameObject(boardLogic.PiecesData[row, col]);
                    if (pieceGameObject != null && pieceGameObject.TryGetComponent(out HexPiece hexPiece))
                    {
                        hexPiece.Init(boardLogic.PiecesData[row, col]);
                        pieces[row, col] = hexPiece;
                    }
                }
            }
        }
        
        private void PlacePiece(Vector2 positionIndex, HexPieceTemplate pieceTemplate = null)
        {
            HexPieceData hexPieceData = boardLogic.PlacePiece((int)positionIndex.x, (int)positionIndex.y, pieceTemplate);
            Assert.IsNotNull(hexPieceData, "tile != null");
            
            GameObject pieceGameObject = boardPresentation.SetPieceGameObject(hexPieceData);
            if (pieceGameObject != null && pieceGameObject.TryGetComponent(out HexPiece hexPiece))
            {
                hexPiece.Init(hexPieceData);
                hexPiece.Show();
                pieces[(int) positionIndex.x, (int) positionIndex.y] = hexPiece;
                
                var neighbors = BoardHelper.GetNeighbours(positionIndex, boardConfig.Width, boardConfig.Height);

                foreach (Vector2 neighbor in neighbors)
                {
                    IHexTile hexTile = tiles[(int)neighbor.x, (int)neighbor.y];
                    
                    if(hexTile.IsOccupied) continue;
                    
                    tiles[(int) neighbor.x, (int) neighbor.y].Show();
                }
                
                /*if(hexPieceData.PieceTemplate.TryGetTrait(out Building building))
                {
                    foreach (var buildingRequirement in building.UnlockRequirementsData)
                    {
                        Vector2 position = positionIndex + buildingRequirement.PositionIndex;
                        PlacePiece(position, buildingRequirement.PieceTemplate);
                    }
                }*/
            }
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