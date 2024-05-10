using System.Collections.Generic;
using Exhale.Scripts.Data;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Exhale.Scripts.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private CellGroundTemplateCollection cellGroundTemplates;
        [SerializeField] private CellBuildingsCollection cellBuildingsTemplates;
        
        [SerializeField] private Transform gridRoot;
        [SerializeField] private Transform groundRoot;
        [SerializeField] private Transform buildingsRoot;
        [SerializeField] private int width = 10; 
        [SerializeField] private int height = 10; 
        [SerializeField] private float rotationSpeed = 100.0f;
        private List<Cell> cells = new ();

        private void Awake()
        {
            Assert.IsNotNull(cellGroundTemplates, "cellGroundTemplates != null");
            Assert.IsNotNull(cellBuildingsTemplates, "cellBuildingsTemplates != null");
        }

        void Start() {
            
            // grid
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    
                    cells.Add(new Cell(x, y));
                    GameObject gridTile = Instantiate(emptyTilePrefab, FromCoordinatesToWorldPosition(x, y), Quaternion.identity);
                    gridTile.transform.parent = gridRoot;
                    gridTile.name = $"Grid tile ({x}, {y})";
                }
            }
            
            // base ground
            for (int x = 3; x < width; x++) {
                for (int y = 3; y < height; y++) {
                    
                    cells.Add(new Cell(x, y));
                    GameObject groundTile = Instantiate(cellGroundTemplates[Random.Range(0, cellGroundTemplates.Count)].BoardPrefab, FromCoordinatesToWorldPosition(x, y), Quaternion.identity);
                    groundTile.transform.parent = gridRoot;
                    groundTile.name = $"Ground tile ({x}, {y})";

                }
            }
            
            // buildings
            var centerCell = GetBoardCenter();
            PlaceBuilding(centerCell);

        }

        private void PlaceBuilding(Vector2 position)
        {
            GameObject buildingTile = Instantiate(cellBuildingsTemplates[Random.Range(0, cellBuildingsTemplates.Count)].BoardPrefab, 
                FromCoordinatesToWorldPosition((int)position.x, (int)position.y), Quaternion.identity);
            buildingTile.transform.parent = buildingsRoot;
            buildingTile.name = $"Ground tile ({position.x}, {position.y})";

        }
        
        private Vector2 GetBoardCenter() {
            return new Vector2(width / 2, height / 2);
        }

        private Vector3 FromCoordinatesToWorldPosition(int x, int y)
        {
            float xOffset = (y % 2 == 1) ? 0.5f : 0f;
            return new Vector3(x + xOffset, 0, y * 0.87f);
        }
    }
}