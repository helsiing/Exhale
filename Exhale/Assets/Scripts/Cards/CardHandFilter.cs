using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Cards.UI
{
    [RequireComponent(typeof(GameHandView))]
    public class CardHandFilter : MonoBehaviour
    {
        private GameHandView handView;
        private readonly ServiceReference<IPlacementService> placementService = new();

        private void Awake() => handView = GetComponent<GameHandView>();

        private void Start()
        {
            if (placementService.Reference == null) return;
            placementService.Reference.OnCardSelected      += OnCardSelected;
            placementService.Reference.OnCardDeselected    += OnCardDeselected;
            placementService.Reference.OnCardLaunchStarted += OnCardLaunchStarted;
        }

        private void OnDestroy()
        {
            if (!placementService.HasCachedReference) return;
            placementService.CachedReference.OnCardSelected      -= OnCardSelected;
            placementService.CachedReference.OnCardDeselected    -= OnCardDeselected;
            placementService.CachedReference.OnCardLaunchStarted -= OnCardLaunchStarted;
        }

        private void OnCardSelected(HexPieceTemplate selectedCard)
        {
            foreach (var leanPivot in handView.HandCards)
            {
                var cardView = leanPivot.GetComponentInChildren<CardView>();
                if (cardView == null) continue;

                // Dim every card that isn't the one being played.
                // The selected card's own highlight is handled by CardView.SetSelected.
                var validity = cardView.Template == selectedCard
                    ? CardValidityState.Valid
                    : CardValidityState.Invalid;
                cardView.SetValidity(validity);
            }
        }

        private void OnCardDeselected() => ResetAllValidity();
        private void OnCardLaunchStarted(HexPieceTemplate _, int2 __) => ResetAllValidity();

        private void ResetAllValidity()
        {
            foreach (var leanPivot in handView.HandCards)
                leanPivot.GetComponentInChildren<CardView>()?.SetValidity(CardValidityState.Valid);
        }
    }
}
