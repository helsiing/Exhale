using System.Linq;
using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts
{
    public class GameplayServicesReporter : ServiceReporter<IService>
    {
        [SerializeField] private CellGroundTemplateCollection cellGroundTemplates;
        public CellGroundTemplateCollection CellGroundTemplates => cellGroundTemplates;
        [SerializeField] private CellBuildingsCollection cellBuildingsTemplates;
        public CellBuildingsCollection CellBuildingsTemplates => CellBuildingsTemplates;
        
        public override void RegisterServices()
        {
            base.RegisterServices();
            RegisterServiceInstance(new Board.TileFactory(cellGroundTemplates.ToList(), cellBuildingsTemplates.ToList()));
        }
    }
}