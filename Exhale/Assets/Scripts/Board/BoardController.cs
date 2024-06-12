using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform gridRoot;
        [SerializeField] private int width = 11; 
        [SerializeField] private int height = 11;
        [SerializeField] private int numBuildings = 5;
        

        private Board board = new();
        
        void Start() {
            
            InitBoard();
            var centerCell = GetBoardCenter();
            
            // base ground
            //var groundCell = board.PlaceGround(centerCell);
            //board.Add(new Cell((int)centerCell.x, (int)centerCell.y, groundCell));
            
            // buildings
            var buildingCell = board.PlaceBuilding(centerCell);
            board.Add(new Cell((int)centerCell.x, (int)centerCell.y, buildingCell));
        }
        
        private Vector2 GetBoardCenter() {
            return new Vector2(width / 2, height / 2);
        }

        private void InitBoard()
        {
            // grid
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    
                    GameObject gridTile = Instantiate(emptyTilePrefab, board.FromCoordinatesToWorldPosition(x, y), Quaternion.identity);
                    gridTile.transform.parent = gridRoot;
                    gridTile.name = $"Grid ({x}, {y})";
                    board.Add(new Cell(x, y, gridTile));
                }
            }
        }
    }
}