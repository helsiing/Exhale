using System.Collections.Generic;
using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class TileFactory : IService
    {
        private List<CellGroundTemplate> groundTemplates;
        private List<CellBuilding> buildingTemplates;
        
        public TileFactory(List<CellGroundTemplate> groundTemplates, List<CellBuilding> buildingTemplates)
        {
            this.groundTemplates = groundTemplates;
            this.buildingTemplates = buildingTemplates;
            Debug.Log("TileFactory Constructor");
        }
        
        public GameObject CreateGroundTile(Vector2 position)
        {
            GameObject tile = Object.Instantiate(groundTemplates[Random.Range(0, groundTemplates.Count)].BoardPrefab, position, Quaternion.identity);
            return tile;
        }
        
        public GameObject CreateBuildingTile(Vector2 position)
        {
            GameObject tile = Object.Instantiate(buildingTemplates[Random.Range(0, buildingTemplates.Count)].BoardPrefab, position, Quaternion.identity);
            return tile;
        }

        public void Dispose()
        {
        }
    }
}