using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exhale.Scripts.Services
{
    public class ServicesBootstrapper : ServiceReporter<IService>
    {
        [SerializeField] private CameraService cameraService;
        [FormerlySerializedAs("gameHandService")] [FormerlySerializedAs("gameService")] [SerializeField] private GameHandHandService gameHandHandService;
        
        public override void RegisterServices()
        {
            base.RegisterServices();
            
            RegisterServiceInstance<IDataService>(new DataService());
            RegisterServiceInstance<ICameraService>(cameraService);
            RegisterServiceInstance<IGameHandService>(gameHandHandService);
            RegisterServiceInstance<IBoardService>(new BoardService(cameraService));
            RegisterServiceInstance<IInventoryService>(new InventoryService());
            
        }
    }
}