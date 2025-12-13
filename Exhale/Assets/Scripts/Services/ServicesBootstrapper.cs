using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Services
{
    public class ServicesBootstrapper : ServiceReporter<IService>
    {
        [SerializeField] private CameraService cameraService;
        [SerializeField] private GameHandService gameHandService;
        
        public override void RegisterServices()
        {
            base.RegisterServices();
            
            RegisterServiceInstance<IDataService>(new DataService());
            RegisterServiceInstance<ICameraService>(cameraService);
            RegisterServiceInstance<IGameHandService>(gameHandService);
            RegisterServiceInstance<IBoardService>(new BoardService(cameraService));
            RegisterServiceInstance<IInventoryService>(new InventoryService());
            
        }
    }
}