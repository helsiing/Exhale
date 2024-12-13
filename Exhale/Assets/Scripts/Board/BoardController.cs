using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private BoardConfig boardConfig;
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform gridRoot;
        
        private Board board = new();
        
        void Start() {
            
            InitBoard();
            var centerCell = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
            
            // base ground
            //var groundCell = board.PlaceGround(centerCell);
            //board.Add(new Cell((int)centerCell.x, (int)centerCell.y, groundCell));
            
            // buildings
            var buildingCell = board.PlaceBuilding(centerCell);
            board.Add(new Cell((int)centerCell.x, (int)centerCell.y, buildingCell));
        }
        
        private void InitBoard()
        {
            // grid
            for (int x = 0; x < boardConfig.Width; x++) {
                for (int y = 0; y < boardConfig.Height; y++) {
                    
                    GameObject gridTile = Instantiate(emptyTilePrefab, board.FromCoordinatesToWorldPosition(x, y), Quaternion.identity);
                    gridTile.transform.parent = gridRoot;
                    gridTile.name = $"Grid ({x}, {y})";
                    board.Add(new Cell(x, y, gridTile));
                }
            }
        }
    }
}