using Exhale.Scripts.External.ServiceLocators;
using Unity.Mathematics;

namespace Exhale.Scripts.Services
{
    public interface IBoardService : IService
    {
        public void TriggerBoardInitialized(int2 startPosition);
    }
    
    public class BoardService : IBoardService
    {
        private readonly ICameraService cameraService;
        
        public BoardService(ICameraService cameraService)
        {
            this.cameraService = cameraService;
        }
        
        public void TriggerBoardInitialized(int2 startPosition)
        {
            cameraService.CenterAtPosition(startPosition);
        }
        
        public void Dispose()
        {
        }
    }
}