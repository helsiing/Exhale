using DG.Tweening;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Cards.UI
{
    public enum CardValidityState { Valid, Invalid }

    public class CardView : MonoBehaviour
    {
        [Header("Validity Feedback")]
        [SerializeField] [Range(0f, 1f)]    private float invalidAlpha     = 0.35f;
        [SerializeField] [Range(0.05f, 1f)] private float validityAnimTime = 0.2f;

        [Header("Selection Feedback")]
        [SerializeField] [Range(0.05f, 1f)] private float selectionAnimTime = 0.15f;

        public HandCard HandCard { get; private set; }
        public HexPieceTemplate Template => HandCard?.Template;
        public CardValidityState Validity { get; private set; } = CardValidityState.Valid;
        public bool IsSelected { get; private set; }

        private bool isClickSource;
        private readonly ServiceReference<IPlacementService> placementService = new();

        public void Initialize(HandCard handCard)
        {
            HandCard = handCard;
        }

        private void Start()
        {
            var service = placementService.Reference;
            if (service == null) return;
            service.OnCardSelected      += OnServiceCardSelected;
            service.OnCardDeselected    += OnServiceCardDeselected;
            service.OnCardLaunchStarted += OnServiceCardLaunchStarted;
        }

        private void OnDestroy()
        {
            if (!placementService.HasCachedReference) return;
            placementService.CachedReference.OnCardSelected      -= OnServiceCardSelected;
            placementService.CachedReference.OnCardDeselected    -= OnServiceCardDeselected;
            placementService.CachedReference.OnCardLaunchStarted -= OnServiceCardLaunchStarted;
        }

        public void SetValidity(CardValidityState validity)
        {
            if (Validity == validity) return;
            Validity = validity;

            if (IsSelected) return;

            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                if(Validity == CardValidityState.Invalid)
                    sr.DOFade(invalidAlpha, validityAnimTime);
                else
                    sr.DOFade(1, validityAnimTime);
            }
        }

        private void SetSelected(bool selected)
        {
            if (IsSelected == selected) return;
            IsSelected = selected;

            GetComponentInParent<CardHandHoverBehavior>()?.SetSelected(selected);
        }

        private void OnMouseDown()
        {
            isClickSource = true;
            placementService.Reference?.TrySelectCard(HandCard);
            isClickSource = false;
        }

        private void OnServiceCardSelected(HandCard _) => SetSelected(isClickSource);
        private void OnServiceCardDeselected()          => SetSelected(false);
        private void OnServiceCardLaunchStarted(HandCard _, int2 __) => SetSelected(false);
    }
}
