using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Scripts.Gameplay
{
    public class BoardSimulation : MonoBehaviour
    {
        //private GameObject selectedTilePrefab;
        private GameObject previewTile;
        private Camera mainCamera;
        
        public delegate void OnPlaceTile(Vector2 position);
        public event OnPlaceTile OnPlaceTileEvent;
        
        void Start()
        {
            //selectedTilePrefab = TileFactory.GetRandomTile(false);
            mainCamera = Camera.main;
            Assert.IsNotNull(mainCamera);
        }

        void Update()
        {
            /*if (Input.GetKeyUp(KeyCode.Tab))
            {
                selectedTilePrefab =TileFactory.GetRandomTile(false);
                ClearTilePreview();
            }*/
            
            //HandleTilePreview();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTile();
            }
        }

        /*private void ClearTilePreview()
        {
            // If the mouse is not over a grid cell, hide the preview tile
            if (previewTile != null)
            {
                Destroy(previewTile);
            }
        }

        private void HandleTilePreview()
        {
            // Create a ray from the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is a grid cell
                GameObject hoveredObject = hit.collider.gameObject;
                if (hoveredObject.CompareTag("GridTile"))
                {
                    Vector3 position = hoveredObject.transform.position;

                    // If there's no preview tile, create one
                    if (previewTile == null)
                    {
                        previewTile = Instantiate(selectedTilePrefab, position, Quaternion.identity);
                        // Make the preview tile semi-transparent
                        SetTileTransparency(previewTile, 0.5f);
                    }
                    else
                    {
                        // Move the preview tile to the new position
                        previewTile.transform.position = position;
                    }
                }
                else
                {
                    // If the mouse is not over a grid cell, hide the preview tile
                    ClearTilePreview();
                }
            }
            else
            {
                // If the raycast doesn't hit anything, hide the preview tile
                ClearTilePreview();
            }
        }*/

        void PlaceTile()
        {
            // Create a ray from the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is a grid cell
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject.CompareTag("GridTile") && 
                    clickedObject.TryGetComponent(out ITileBoardPositionProvider tileBoardPositionProvider))
                {
                    OnPlaceTileEvent?.Invoke(tileBoardPositionProvider.BoardPosition);
                    // Destroy the preview tile to avoid duplication
                    if (previewTile != null)
                    {
                        Destroy(previewTile);
                    }
                }
            }
        }
    }
}