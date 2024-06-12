using System.Collections.Generic;
using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class TileFactory : IService
    {
        private List<CellGroundTemplate> groundTemplates;
        private List<CellBuildingTemplate> buildingsTemplates;
        
        public TileFactory(List<CellGroundTemplate> groundTemplates, List<CellBuildingTemplate> buildingsTemplates)
        {
            this.groundTemplates = groundTemplates;
            this.buildingsTemplates = buildingsTemplates;
        }
        
        public GameObject GetRandomGroundTile(bool shouldInstantiate)
        {
            int count = groundTemplates.Count;
            if (shouldInstantiate)
            {
                return Object.Instantiate(groundTemplates[Random.Range(0, count)].BoardPrefab);
            }
            else
            {
                return groundTemplates[Random.Range(0, count)].BoardPrefab;
            }
        }
        
        public GameObject GetRandomBuildingTile(bool shouldInstantiate)
        {
            int count = buildingsTemplates.Count;
            if (shouldInstantiate)
            {
                return Object.Instantiate(buildingsTemplates[Random.Range(0, buildingsTemplates.Count)].BoardPrefab);
            }
            else
            {
                return buildingsTemplates[Random.Range(0, count)].BoardPrefab;
            }
        }

        public void Dispose()
        {
        }
    }
}