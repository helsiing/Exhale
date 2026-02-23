using Exhale.Board;
using Exhale.Plugins.ServiceLocators;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Scripts.Services
{
    public interface ICameraService : IService
    {
        public void CenterAtPosition(int2 position);
    }
    
    public class CameraService : MonoBehaviour, ICameraService
    {
        private CameraController cameraController;
        
        private void Awake()
        {
            cameraController = FindFirstObjectByType<CameraController>();
        }
        public void Dispose()
        {
        }

        public void CenterAtPosition(int2 position)
        {
            cameraController.CenterAtPosition(position);
        }
    }
}