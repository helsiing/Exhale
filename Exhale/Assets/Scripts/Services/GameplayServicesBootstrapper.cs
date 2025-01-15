using Exhale.Scripts.External.ServiceLocators;

namespace Exhale.Services
{
    public class GameplayServicesBootstrapper : ServiceReporter<IService>
    {
        public override void RegisterServices()
        {
            base.RegisterServices();
            
            IInventoryService inventoryService = new InventoryService();
            RegisterServiceInstance(inventoryService);

            IBoardService boardService = new BoardService();
            RegisterServiceInstance(boardService);
            
            RegisterServiceInstance<IGameplayService>(new GameplayService(inventoryService, boardService));
            
        }
    }
}