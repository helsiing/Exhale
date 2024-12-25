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
            Vector2 centerCellBoardPosition = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
         
            boardLogic.InitBoard(boardConfig.Width, boardConfig.Height);
            boardPresentation.DrawBoard(boardLogic.Tiles);
            
            HexTileData hexTileData = new HexTileData(centerCellBoardPosition, HexTileType.Building);
            HexTile hexTile = PlaceTile(hexTileData);
        }

        private HexTile PlaceTile(HexTileData hexTileData)
        {
            HexTileData hexTile = boardLogic.PlaceTile(hexTileData.Position, hexTileData.Type);
            Assert.IsNotNull(hexTile, "tile != null");
            return boardPresentation.DrawTile(hexTile);
        }
        
        void OnPlaceTile(Vector2 position)
        {
            HexTileData hexTileData = new HexTileData(position, HexTileType.Building);
            HexTile hexTile = PlaceTile(hexTileData);
        }

        private void OnDestroy()
        {
            boardSimulation.OnPlaceTileEvent -= OnPlaceTile;
        }
    }
}