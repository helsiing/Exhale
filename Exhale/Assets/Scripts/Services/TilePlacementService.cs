using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Scripts.Board
{
    public class TilePlacementService : MonoBehaviour, IService
    {
        private GameObject selectedTilePrefab;
        private GameObject previewTile;
        private readonly ServiceReference<TileFactory> tileFactoryService = new();
        private Camera mainCamera;

        void Start()
        {
            selectedTilePrefab = tileFactoryService.Reference.GetRandomGroundTile(false);
            mainCamera = Camera.main;
            Assert.IsNotNull(mainCamera);
        }

        void Update()
        {

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                selectedTilePrefab = tileFactoryService.Reference.GetRandomGroundTile(false);
                ClearTilePreview();
            }
            
            HandleTilePreview();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTile();
            }
        }

        private void ClearTilePreview()
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
        }

        void PlaceTile()
        {
            // Create a ray from the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is a grid cell
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject.CompareTag("GridTile"))
                {
                    // Place the selected tile at the clicked position
                    Vector3 position = clickedObject.transform.position;
                    GameObject newTile = Instantiate(selectedTilePrefab, position, Quaternion.identity);
                    newTile.transform.parent = clickedObject.transform;

                    // Destroy the preview tile to avoid duplication
                    if (previewTile != null)
                    {
                        Destroy(previewTile);
                    }
                }
            }
        }

        public void SelectTile(int tileType)
        {
            // Set the selected tile prefab based on the tileType (0: Wood, 1: Sand, etc.)
            /*switch (tileType)
            {
                case 0:
                    selectedTilePrefab = woodTilePrefab;
                    break;
                case 1:
                    selectedTilePrefab = sandTilePrefab;
                    break;
                case 2:
                    selectedTilePrefab = stoneTilePrefab;
                    break;
                case 3:
                    selectedTilePrefab = waterTilePrefab;
                    break;
                // Add cases for other tile types
            }*/

            // Update the preview tile if it exists
            if (previewTile != null)
            {
                Destroy(previewTile);
                previewTile = Instantiate(selectedTilePrefab, previewTile.transform.position, Quaternion.identity);
                SetTileTransparency(previewTile, 0.5f);
            }
        }

        void SetTileTransparency(GameObject tile, float alpha)
        {
            // Set the transparency of the tile (assuming it has a Renderer component)
            Renderer renderer = tile.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }

        public void Dispose()
        {
            
        }
    }
}