using Exhale.Scripts.External.ServiceLocators;

namespace Exhale.Scripts.Services
{
    public class GameplayServicesBootstrapper : ServiceReporter<IService>
    {
        public override void RegisterServices()
        {
            base.RegisterServices();

            RegisterServiceInstance<IBoardService>(new BoardService());
        }
    }
}