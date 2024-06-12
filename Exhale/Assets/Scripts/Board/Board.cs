using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class Board
    {
        private ServiceReference<TileFactory> tileFactory = new();
        private List<Cell> cells = new ();
        public List<Cell> Cells => cells;

        public void Add(Cell cell)
        {
            cells.Add(cell);
        }

        private Cell Get(Vector2 position)
        {
            return cells.FirstOrDefault(x => x.Position.Equals(position));
        }
        
        public GameObject PlaceGround(Vector2 position)
        {
            GameObject groundTile =
                tileFactory.Reference.GetRandomGroundTile(true);
            var cell = Get(position);
            groundTile.transform.parent = cell.CellObject.transform;
            groundTile.transform.position = FromCoordinatesToWorldPosition(position);
            groundTile.transform.rotation = Quaternion.identity;
            return groundTile;
        }
        
        public GameObject PlaceBuilding(Vector2 position)
        {
            GameObject buildingTile =
                tileFactory.Reference.GetRandomBuildingTile(true);
            buildingTile.transform.position = FromCoordinatesToWorldPosition(position);
            var cell = Get(position);
            buildingTile.transform.parent = cell.CellObject.transform;
            buildingTile.transform.rotation = Quaternion.identity;
            return buildingTile;
        }

        private Vector3 FromCoordinatesToWorldPosition(Vector2 position)
        {
            return FromCoordinatesToWorldPosition((int)position.x, (int)position.y);
        }
        
        public Vector3 FromCoordinatesToWorldPosition(int x, int y)
        {
            float xOffset = (y % 2 == 1) ? 0.5f : 0f;
            return new Vector3(x + xOffset, 0, y * 0.87f);
        }
    }
}