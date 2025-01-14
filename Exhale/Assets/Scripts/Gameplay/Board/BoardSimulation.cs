using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Scripts.Gameplay
{
    public class BoardSimulation : MonoBehaviour
    {
        private GameObject previewTile;
        private Camera mainCamera;
        
        public Action<Vector2> OnTileClickedEvent;
        
        void Start()
        {
            mainCamera = Camera.main;
            Assert.IsNotNull(mainCamera);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClicked();
            }
        }
        
        void OnMouseClicked()
        {
            // Create a ray from the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is a grid cell
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject.CompareTag("GridTile") && 
                    clickedObject.TryGetComponent(out IBoardPositionProvider tileBoardPositionProvider))
                {
                    OnTileClickedEvent?.Invoke(tileBoardPositionProvider.PositionIndex);
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