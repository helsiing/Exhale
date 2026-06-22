using System;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Exhale.Scripts.Services
{
    public interface IPlacementService : IService
    {
        PlacementState State { get; }
        HandCard SelectedCard { get; }
        int2 ConfirmedTilePosition { get; }
        int2? HoveredValidTilePosition { get; }

        event Action<HandCard> OnCardSelected;
        event Action OnCardDeselected;
        event Action<HandCard, int2> OnCardLaunchStarted;
        event Action<HandCard, int2> OnCardLanded;

        bool TrySelectCard(HandCard card);
        bool TryConfirmTile(int2 pos, Entity tileEntity);
        void Cancel();
        bool IsCardValidForTile(HandCard card, int2 tilePos);
        void NotifyCardLanded();
        void NotifyTileHovered(int2? tilePos);
    }
}
