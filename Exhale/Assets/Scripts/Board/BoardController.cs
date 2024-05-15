using System.Collections.Generic;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        
        [SerializeField] private Transform gridRoot;
        [SerializeField] private Transform groundRoot;
        [SerializeField] private Transform buildingsRoot;
        [SerializeField] private int width = 11; 
        [SerializeField] private int height = 11; 
        private List<Cell> cells = new ();
        private ServiceReference<TileFactory> tileFactory = new();

        
        void Start() {
            
            var centerCell = GetBoardCenter();
            Debug.Log(centerCell);
            
            // grid
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    
                    GameObject gridTile = Instantiate(emptyTilePrefab, FromCoordinatesToWorldPosition(x, y), Quaternion.identity);
                    gridTile.transform.parent = gridRoot;
                    gridTile.name = $"Grid tile ({x}, {y})";
                    cells.Add(new Cell(x, y, gridTile));

                }
            }
            
            // base ground
            GameObject groundTile =
                tileFactory.Reference.CreateGroundTile(centerCell);
            groundTile.transform.parent = groundRoot;
            groundTile.transform.position = FromCoordinatesToWorldPosition(centerCell);
            groundTile.name = $"Ground tile ({centerCell.x}, {centerCell.y})";
            cells.Add(new Cell((int)centerCell.x, (int)centerCell.y, groundTile));
            
            // buildings
            //var buildingCell = PlaceBuilding(centerCell);
            //cells.Add(new Cell((int)centerCell.x, (int)centerCell.y, buildingCell));

        }

        private GameObject PlaceBuilding(Vector2 position)
        {
            GameObject buildingTile =
                tileFactory.Reference.CreateBuildingTile(FromCoordinatesToWorldPosition((int)position.x, (int)position.y));
            buildingTile.transform.parent = buildingsRoot;
            buildingTile.name = $"Building tile ({position.x}, {position.y})";
            return buildingTile;
        }
        
        private Vector2 GetBoardCenter() {
            return new Vector2(width / 2, height / 2);
        }

        private Vector3 FromCoordinatesToWorldPosition(int x, int y)
        {
            float xOffset = (y % 2 == 1) ? 0.5f : 0f;
            return new Vector3(x + xOffset, 0, y * 0.87f);
        }
        
        private Vector3 FromCoordinatesToWorldPosition(Vector2 position)
        {
            return FromCoordinatesToWorldPosition((int)position.x, (int)position.y);
        }
    }
}