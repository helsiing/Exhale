using Exhale.Scripts.Board;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts
{
    public class GameplayServicesReporter : ServiceReporter<IService>
    {
        //[SerializeField] private TilePlacementService tilePlacementService;
        
        public override void RegisterServices()
        {
            //base.RegisterServices();
            //RegisterServiceInstance(tilePlacementService);  
        }
    }
}