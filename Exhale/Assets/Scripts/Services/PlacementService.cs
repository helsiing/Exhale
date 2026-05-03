using System;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Scripts.Services
{
    public enum PlacementState { Idle, TileHovered, TileArmed, CardLaunching, Resolving }

    public interface IPlacementService : IService
    {
        PlacementState State { get; }
        int2 ArmedTilePosition { get; }

        event Action<int2> OnTileHovered;
        event Action<int2, Entity> OnTileArmed;
        event Action OnTileDisarmed;
        event Action<HexPieceTemplate, int2> OnCardLaunchStarted;
        event Action<HexPieceTemplate, int2> OnCardLanded;

        void NotifyHover(int2 pos);
        void NotifyHoverEnd();
        bool TryArmTile(int2 pos, Entity tileEntity);
        bool TryLaunchCard(HexPieceTemplate card);
        void Cancel();
        bool IsCardValidForTile(HexPieceTemplate card, int2 tilePos);
        void NotifyCardLanded();
    }

    public class PlacementService : IPlacementService
    {
        public PlacementState State { get; private set; } = PlacementState.Idle;
        public int2 ArmedTilePosition { get; private set; }

        private Entity armedTileEntity;
        private HexPieceTemplate launchingCard;

        public event Action<int2> OnTileHovered;
        public event Action<int2, Entity> OnTileArmed;
        public event Action OnTileDisarmed;
        public event Action<HexPieceTemplate, int2> OnCardLaunchStarted;
        public event Action<HexPieceTemplate, int2> OnCardLanded;

        public void NotifyHover(int2 pos)
        {
            if (State != PlacementState.Idle && State != PlacementState.TileHovered) return;
            State = PlacementState.TileHovered;
            OnTileHovered?.Invoke(pos);
        }

        public void NotifyHoverEnd()
        {
            if (State != PlacementState.TileHovered) return;
            State = PlacementState.Idle;
        }

        public bool TryArmTile(int2 pos, Entity tileEntity)
        {
            if (State == PlacementState.CardLaunching || State == PlacementState.Resolving)
                return false;

            if (State == PlacementState.TileArmed)
                OnTileDisarmed?.Invoke();

            ArmedTilePosition = pos;
            armedTileEntity = tileEntity;
            State = PlacementState.TileArmed;
            OnTileArmed?.Invoke(pos, tileEntity);
            Debug.Log($"[PlacementService] Tile armed at {pos}");
            return true;
        }

        public bool TryLaunchCard(HexPieceTemplate card)
        {
            if (State != PlacementState.TileArmed) return false;
            if (card == null || !IsCardValidForTile(card, ArmedTilePosition)) return false;

            launchingCard = card;
            State = PlacementState.CardLaunching;
            OnCardLaunchStarted?.Invoke(card, ArmedTilePosition);
            Debug.Log($"[PlacementService] Card launching: {card.name} → {ArmedTilePosition}");
            return true;
        }

        public void Cancel()
        {
            if (State is PlacementState.CardLaunching or PlacementState.Resolving or PlacementState.Idle)
                return;

            State = PlacementState.Idle;
            OnTileDisarmed?.Invoke();
            Debug.Log("[PlacementService] Cancelled.");
        }

        public bool IsCardValidForTile(HexPieceTemplate card, int2 tilePos)
        {
            // Placeholder: any card that can go on the board is valid.
            // Step 8 (CardHandFilter) will add terrain/cost constraints.
            return card != null && card.HasTrait<BoardObject>();
        }

        public void NotifyCardLanded()
        {
            if (State != PlacementState.CardLaunching) return;

            var card = launchingCard;
            var pos = ArmedTilePosition;
            launchingCard = null;
            State = PlacementState.Resolving;
            OnCardLanded?.Invoke(card, pos);
            State = PlacementState.Idle;
        }

        public void Dispose() { }
    }
}
