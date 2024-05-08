using System;
using Exhale.Scripts.Data;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Exhale.Scripts.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private HexCellGroundTemplateCollection cellGroundTemplates;
        [SerializeField] private HexCellBuildingCollection cellBuildingsTemplates;
        
        [SerializeField] private Transform tilesRoot;
        [SerializeField] private int width = 10; 
        [SerializeField] private int height = 10; 
        [SerializeField] private float rotationSpeed = 100.0f;

        private void Awake()
        {
            Assert.IsNotNull(cellGroundTemplates, "cellGroundTemplates != null");
        }

        void Start() {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    
                    GameObject ground = Instantiate(emptyTilePrefab, FromCoordinatesToWorldPosition(x, y), Quaternion.identity);
                    ground.transform.parent = tilesRoot;
                }
            }
            
            var centerCell = GetBoardCenter();
            GameObject building = Instantiate(cellBuildingsTemplates[Random.Range(0, cellBuildingsTemplates.Count)].BoardPrefab, 
                FromCoordinatesToWorldPosition((int)centerCell.x, (int)centerCell.y), Quaternion.identity);
            building.transform.parent = tilesRoot;

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