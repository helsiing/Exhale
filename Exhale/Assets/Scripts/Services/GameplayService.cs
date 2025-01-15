
using Data.Yield;
using Exhale.Gameplay;
using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;

namespace Exhale.Services
{
    public interface IGameplayService : IService
    {
    }
    
    public class GameplayService : IGameplayService
    {
        private IInventoryService inventoryService;
        private IBoardService boardService;
        
        public GameplayService(IInventoryService inventoryService, IBoardService boardService)
        {
            this.inventoryService = inventoryService;
            this.boardService = boardService;
            
            boardService.OnPiecePlaced += OnPiecePlaced;
        }

        private void OnPiecePlaced(IHexPiece piece)
        {
            // check the yields
            if (!piece.PieceTemplate.TryGetTrait(out Yield yield)) return;
            
            foreach (YieldData yieldData in yield.Yields)
            {
                inventoryService.AddYield(yieldData.YieldTemplate, yieldData.Amount);
            }
        }

        public void Dispose()
        {
            boardService.OnPiecePlaced -= OnPiecePlaced;
        }
    }
}