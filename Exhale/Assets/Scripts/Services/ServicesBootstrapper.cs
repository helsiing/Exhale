using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Services
{
    public class ServicesBootstrapper : ServiceReporter<IService>
    {
        [SerializeField] private CameraService cameraService;
        
        public override void RegisterServices()
        {
            base.RegisterServices();
            
            RegisterServiceInstance<ICameraService>(cameraService);

            IBoardService boardService = new BoardService(cameraService);
            RegisterServiceInstance(boardService);
        }
    }
}