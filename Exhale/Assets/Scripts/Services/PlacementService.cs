using System;
using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.Scripts.Services
{
    public enum PlacementState { Idle, CardSelected, CardLaunching, Resolving }

    public class PlacementService : IPlacementService
    {
        public PlacementState State { get; private set; } = PlacementState.Idle;
        public HandCard SelectedCard { get; private set; }
        public int2 ConfirmedTilePosition { get; private set; }

        private HandCard launchingCard;

        public event Action<HandCard> OnCardSelected;
        public event Action OnCardDeselected;
        public event Action<HandCard, int2> OnCardLaunchStarted;
        public event Action<HandCard, int2> OnCardLanded;

        public bool TrySelectCard(HandCard card)
        {
            if (State is PlacementState.CardLaunching or PlacementState.Resolving)
                return false;

            // Toggle: clicking the already-selected card deselects it.
            if (State == PlacementState.CardSelected && SelectedCard == card)
            {
                Deselect();
                return true;
            }

            // Swap: a different card was clicked while one was already selected.
            if (State == PlacementState.CardSelected)
                Deselect();

            SelectedCard = card;
            State = PlacementState.CardSelected;
            OnCardSelected?.Invoke(card);
            return true;
        }

        public bool TryConfirmTile(int2 pos, Entity tileEntity)
        {
            if (State != PlacementState.CardSelected) return false;
            if (SelectedCard == null || !IsCardValidForTile(SelectedCard, pos)) return false;

            ConfirmedTilePosition = pos;
            launchingCard = SelectedCard;
            SelectedCard = null;
            State = PlacementState.CardLaunching;
            OnCardLaunchStarted?.Invoke(launchingCard, pos);
            return true;
        }

        public void Cancel()
        {
            if (State is PlacementState.CardLaunching or PlacementState.Resolving or PlacementState.Idle)
                return;

            Deselect();
        }

        public bool IsCardValidForTile(HandCard card, int2 tilePos)
        {
            // Placeholder — terrain/cost constraints will be added here.
            return card?.Template.HasTrait<BoardObject>() == true;
        }

        public void NotifyCardLanded()
        {
            if (State != PlacementState.CardLaunching) return;

            var card = launchingCard;
            var pos  = ConfirmedTilePosition;
            launchingCard = null;
            State = PlacementState.Resolving;
            OnCardLanded?.Invoke(card, pos);
            State = PlacementState.Idle;
        }

        public void Dispose() { }

        private void Deselect()
        {
            SelectedCard = null;
            State = PlacementState.Idle;
            OnCardDeselected?.Invoke();
        }
    }
}
