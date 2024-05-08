using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class BoardController : MonoBehaviour
    {
        public Transform tilesRoot;
        public GameObject[] tilePrefabs;
        public GameObject groundPrefab;
        public int width = 10; // Width of the map
        public int height = 10; // Height of the map
        public float rotationSpeed = 100.0f;

        void Start() {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    
                    GameObject ground = Instantiate(groundPrefab, FromCoordinatesToWorldPosition(x, y), Quaternion.identity);
                    ground.transform.parent = tilesRoot;
                }
            }
            
            var centerCell = GetBoardCenter();
            GameObject hex = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], FromCoordinatesToWorldPosition((int)centerCell.x, (int)centerCell.y), Quaternion.identity);
            hex.transform.parent = tilesRoot;

        }
        
        private Vector2 GetBoardCenter() {
            return new Vector2(width / 2, height / 2);
        }

        private Vector3 FromCoordinatesToWorldPosition(int x, int y)
        {
            float xOffset = (y % 2 == 1) ? 0.5f : 0f;
            return new Vector3(x + xOffset, 0, y * 0.75f);
        }
    }
}