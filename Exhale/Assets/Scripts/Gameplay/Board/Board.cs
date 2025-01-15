using System.Linq;
using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using Exhale.Services;
using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Gameplay
{
    public interface IBoard
    {
        public int Width { get; }
        public int Height { get; }
    }

    [RequireComponent(typeof(BoardSimulation))]
    [RequireComponent(typeof(BoardPresentation))]
    public class Board : MonoBehaviour, IBoard
    {
        [SerializeField] private BoardConfig boardConfig;
        
        private IHexTile[,] tiles;
        private IHexPiece[,] pieces;
        
        private BoardSimulation boardSimulation;
        private BoardPresentation boardPresentation;
        private IBoardLogic boardLogic;
        
        private readonly ServiceReference<IBoardService> boardService = new();
        private readonly ServiceReference<IInventoryService> inventoryService = new();

        public int Width => boardConfig.Width;
        public int Height => boardConfig.Height;

        private void Awake()
        {
            TryGetComponent(out boardSimulation);
            TryGetComponent(out boardPresentation);
            
            boardSimulation.OnTileClickedEvent += OnTileClicked;
        }

        private void Start()
        {
            boardLogic = new BoardLogic();
            
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
            
            boardLogic.Init(boardConfig.Width, boardConfig.Height);
            boardPresentation.Init(this);
            
            for (int x = 0; x < boardConfig.Width; x++)
            {
                for (int y = 0; y < boardConfig.Height; y++)
                {
                    var tileData = boardLogic.GetTileAt(x, y);
                    Assert.IsNotNull(tileData, "tileData != null");
                    
                    var tileGameObject = boardPresentation.SetTileGameObject(tileData);
                    if (tileGameObject != null && tileGameObject.TryGetComponent(out IHexTile hexTile))
                    {
                        hexTile.Init(tileData);
                        hexTile.Hide();
                        SetTileAt(x, y, hexTile);
                    }
                }
            }
        }

        public IHexTile GetTileAt(int x, int y)
        {
            return BoardHelper.IsWithinBounds(Width, Height, x, y) ? tiles[x, y] : null;
        }

        public bool SetTileAt(int x, int y, IHexTile hexTile)
        {
            if (!BoardHelper.IsWithinBounds(Width, Height, x, y)) return false;
            tiles[x, y] = hexTile;
            return true;
        }
        
        public IHexPiece GetPieceAt(int x, int y)
        {
            return BoardHelper.IsWithinBounds(Width, Height, x, y) ? pieces[x, y] : null;
        }

        public bool SetPieceAt(int x, int y, IHexPiece hexPiece)
        {
            if (!BoardHelper.IsWithinBounds(Width, Height, x, y)) return false;
            
            pieces[x, y] = hexPiece;
            hexPiece.Show();
            
            foreach (var neighbourTile in BoardHelper.GetNeighbours(new Vector2(x, y), Width, Height)
                         .Select(neighbour => GetTileAt((int)neighbour.x, (int)neighbour.y)))
            {
                neighbourTile.Show();
            }
            
            return true;
        }
        
        private void PlacePiece(Vector2 positionIndex, HexPieceTemplate pieceTemplate = null)
        {
            // check if the tile is empty
            HexTileData hexTileData = boardLogic.GetTileAt((int)positionIndex.x, (int)positionIndex.y);
            if (hexTileData.HasPiece)
            {
                Debug.LogError($"Tile at ({positionIndex.x}, {positionIndex.y}) is already occupied.");
                return;
            }
            
            HexPieceData hexPieceData = boardLogic.PlacePiece((int)positionIndex.x, (int)positionIndex.y, pieceTemplate);
            Assert.IsNotNull(hexPieceData, "hexPieceData != null");
            GameObject pieceGameObject = boardPresentation.SetPieceGameObject(hexPieceData);
            if (pieceGameObject != null && pieceGameObject.TryGetComponent(out HexPiece hexPiece))
            {
                hexPiece.Init(hexPieceData);
                SetPieceAt((int) positionIndex.x, (int) positionIndex.y, hexPiece);
                
                boardService.Reference.OnPiecePlaced?.Invoke(hexPieceData);
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