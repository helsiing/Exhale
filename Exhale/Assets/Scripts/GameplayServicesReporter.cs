using System.Linq;
using Exhale.Scripts.Board;
using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts
{
    public class GameplayServicesReporter : ServiceReporter<IService>
    {
        [SerializeField] private TilePlacementService tilePlacementService;
        
        public override void RegisterServices()
        {
            base.RegisterServices();
            RegisterServiceInstance(new TileFactory(CellGroundTemplateCollection.Values.ToList(), CellBuildingsTemplateCollection.Values.ToList()));
            RegisterServiceInstance(tilePlacementService);  
        }
    }
}