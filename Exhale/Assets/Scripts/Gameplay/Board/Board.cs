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
            
            boardSimulation.OnPlaceTileEvent += OnPlaceTile;
        }

        private void Start() 
        {
            Vector2 centerCell = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
         
            boardLogic.InitBoard(boardConfig.Width, boardConfig.Height);
            boardPresentation.DrawBoard(boardLogic.Tiles);
            
            TileData tileData = new TileData(centerCell, TileType.Building);
            Tile tile = PlaceTile(tileData);

        }

        private Tile PlaceTile(TileData tileData)
        {
            TileData tile = boardLogic.PlaceTile(tileData.Position, tileData.Type);
            Assert.IsNotNull(tile, "tile != null");
            return boardPresentation.DrawTile(tile);
        }
        
        void OnPlaceTile(Vector2 position)
        {
            TileData tileData = new TileData(position, TileType.Building);
            Tile tile = PlaceTile(tileData);
        }

        private void OnDestroy()
        {
            boardSimulation.OnPlaceTileEvent -= OnPlaceTile;
        }
    }
}