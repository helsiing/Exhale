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
        [SerializeField] private Camera camera;

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
            
            // Position the camera
            Vector3 centerCellWorldPosition = BoardHelper.FromCoordinatesToWorldPosition(centerCellBoardPosition);
            camera.transform.position = new Vector3(centerCellWorldPosition.x, 10f, centerCellWorldPosition.z - 10f); // Adjust Y and Z for your scene setup
            camera.transform.LookAt(centerCellWorldPosition); // Make the camera look at the grid center
            
            TileData tileData = new TileData(centerCellBoardPosition, TileType.Building);
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