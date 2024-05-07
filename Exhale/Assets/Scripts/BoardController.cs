using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class BoardController : MonoBehaviour
    {
        #region Board
        public GameObject[] tilePrefabs;
        public int width = 10; // Width of the map
        public int height = 10; // Height of the map
        public float rotationSpeed = 100.0f;
        #endregion
        
        #region Camera
        public Camera camera;
        public float zoomSpeed = 10f; // Speed of zoom
        public float minFieldOfView = 35f; 
        public float maxFieldOfView = 100f;
        private float currentDist; // Current distance from target

        #endregion

        void Start() {
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < height; z++) {
                    
                    // Offset coordinates for every other row
                    float xOffset = (z % 2 == 1) ? 0.5f : 0f;

                    // Instantiate a new hex tile GameObject
                    GameObject hex = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], new Vector3(x + xOffset, 0, z * 0.75f), Quaternion.identity);
                    hex.transform.parent = this.transform;
                }
            }
            
            currentDist = Vector3.Distance(transform.position, Vector3.zero); // Assuming target is at world origin

        }
        void Update() {
            if (Input.GetMouseButton(0)) {
                float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                transform.Rotate(0, -rotationX, 0);
            }
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentDist = Mathf.Clamp(currentDist - scroll * zoomSpeed, minFieldOfView, maxFieldOfView);
            camera.fieldOfView = currentDist;
        }
    }
}