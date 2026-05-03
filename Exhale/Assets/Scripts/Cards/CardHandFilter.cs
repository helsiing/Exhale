using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Services;
using Unity.Entities;
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
            placementService.Reference.OnTileArmed    += OnTileArmed;
            placementService.Reference.OnTileDisarmed += OnTileDisarmed;
        }

        private void OnDestroy()
        {
            if (placementService.Reference == null) return;
            placementService.Reference.OnTileArmed    -= OnTileArmed;
            placementService.Reference.OnTileDisarmed -= OnTileDisarmed;
        }

        private void OnTileArmed(int2 pos, Entity _)
        {
            var service = placementService.Reference;
            foreach (var leanPivot in handView.HandCards)
            {
                var cardView = leanPivot.GetComponentInChildren<CardView>();
                if (cardView == null) continue;

                var state = service.IsCardValidForTile(cardView.Template, pos)
                    ? CardValidityState.Valid
                    : CardValidityState.Invalid;
                cardView.SetValidity(state);
            }
        }

        private void OnTileDisarmed()
        {
            foreach (var leanPivot in handView.HandCards)
                leanPivot.GetComponentInChildren<CardView>()?.SetValidity(CardValidityState.Valid);
        }
    }
}
