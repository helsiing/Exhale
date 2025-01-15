using Exhale.Scripts.External.ServiceLocators;

namespace Exhale.Services
{
    public class GameplayServicesBootstrapper : ServiceReporter<IService>
    {
        public override void RegisterServices()
        {
            base.RegisterServices();

            RegisterServiceInstance<IBoardService>(new BoardService());
            RegisterServiceInstance<IInventoryService>(new InventoryService());
        }
    }
}