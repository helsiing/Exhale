using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Services
{
    public class ServicesBootstrapper : ServiceReporter<IService>
    {
        [SerializeField] private CameraService cameraService;
        [SerializeField] private GameService gameService;
        
        public override void RegisterServices()
        {
            base.RegisterServices();
            
            RegisterServiceInstance<ICameraService>(cameraService);
            RegisterServiceInstance<IGameService>(gameService);

            IBoardService boardService = new BoardService(cameraService);
            RegisterServiceInstance(boardService);
            
            IInventoryService inventoryService = new InventoryService();
            RegisterServiceInstance(inventoryService);
        }
    }
}