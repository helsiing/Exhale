using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Exhale.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Cards.UI
{
    /// <summary>
    /// Shows a ghost preview of the selected card's board piece when hovering
    /// over valid placement tiles. Attach to any persistent scene GameObject.
    /// </summary>
    public class CardPlacementPreviewController : MonoBehaviour
    {
        [Tooltip("Vertical offset above the tile surface for the ghost piece.")]
        [SerializeField] private float hoverHeight = 0.5f;

        [Tooltip("Tint applied to all renderers on the ghost piece via MaterialPropertyBlock.")]
        [SerializeField] private Color ghostTint = new(0.7f, 0.9f, 1.0f, 1f); // pale blue-white

        private readonly ServiceReference<IPlacementService> placementService = new();

        private GameObject ghostInstance;
        private MaterialPropertyBlock propertyBlock;
        private int2? lastHoveredPos;

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            var service = placementService.Reference;
            if (service == null) return;
            service.OnCardSelected   += OnCardSelected;
            service.OnCardDeselected += OnCardDeselected;
            service.OnCardLanded     += OnCardLanded;
        }

        private void OnDisable()
        {
            if (!placementService.HasCachedReference) return;
            var service = placementService.CachedReference;
            service.OnCardSelected   -= OnCardSelected;
            service.OnCardDeselected -= OnCardDeselected;
            service.OnCardLanded     -= OnCardLanded;
            DestroyGhost();
        }

        private void Update()
        {
            var service = placementService.Reference;
            if (service == null || ghostInstance == null) return;

            var hoveredPos = service.HoveredValidTilePosition;

            if (hoveredPos == null)
            {
                ghostInstance.SetActive(false);
                lastHoveredPos = null;
                return;
            }

            if (hoveredPos.Equals(lastHoveredPos))
                return; // same tile — no need to reposition

            lastHoveredPos = hoveredPos;
            var worldPos = (Vector3)BoardHelper.HexToWorldPosition(hoveredPos.Value);
            worldPos.y += hoverHeight;
            ghostInstance.transform.position = worldPos;
            ghostInstance.SetActive(true);
        }

        private void OnCardSelected(HandCard card)
        {
            DestroyGhost();

            if (!card.Template.TryGetTrait<BoardObject>(out var boardObject)) return;
            if (boardObject.Prefab == null) return;

            ghostInstance = Instantiate(boardObject.Prefab);
            ghostInstance.SetActive(false);
            ApplyGhostTint(ghostInstance);
        }

        private void OnCardDeselected()  => DestroyGhost();
        private void OnCardLanded(HandCard _, int2 __)  => DestroyGhost();

        private void ApplyGhostTint(GameObject ghost)
        {
            propertyBlock.SetColor("_BaseColor", ghostTint);
            foreach (var r in ghost.GetComponentsInChildren<Renderer>(true))
                r.SetPropertyBlock(propertyBlock);
        }

        private void DestroyGhost()
        {
            if (ghostInstance != null)
            {
                Destroy(ghostInstance);
                ghostInstance = null;
            }
            lastHoveredPos = null;
        }
    }
}
