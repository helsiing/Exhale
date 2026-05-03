using DG.Tweening;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using UnityEngine;

namespace Exhale.Cards.UI
{
    public enum CardValidityState { Valid, Invalid }

    public class CardView : MonoBehaviour
    {
        [Header("Validity Feedback")]
        [SerializeField] [Range(0f, 1f)]    private float invalidAlpha     = 0.35f;
        [SerializeField] [Range(0.05f, 1f)] private float validityAnimTime = 0.2f;

        public HexPieceTemplate Template { get; private set; }
        public CardValidityState Validity { get; private set; } = CardValidityState.Valid;

        private readonly ServiceReference<IPlacementService> placementService = new();

        public void Initialize(HexPieceTemplate template)
        {
            Template = template;
        }

        public void SetValidity(CardValidityState validity)
        {
            if (Validity == validity) return;
            Validity = validity;

            var targetColor = validity == CardValidityState.Valid
                ? Color.white
                : new Color(1f, 1f, 1f, invalidAlpha);

            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.DOColor(targetColor, validityAnimTime);
        }

        private void OnMouseDown()
        {
            placementService.Reference?.TryLaunchCard(Template);
        }
    }
}
