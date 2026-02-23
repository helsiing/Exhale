using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.Data;
using Exhale.Plugins.ServiceLocators;

namespace Exhale.Scripts.Services
{
    public interface IDataService : IService
    {
        public List<HexPieceTemplate> GetHexTilesAvailable();
    }
    
    public class DataService : IDataService
    {
        public List<HexPieceTemplate> GetHexTilesAvailable()
        {
            return HexPieceTemplateCollection.Values.ToList();
        }
        
        public void Dispose()
        {
            
        }
    }
}