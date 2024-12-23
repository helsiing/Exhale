using System;
using Exhale.Scripts.Data;
using Exhale.Scripts.Utils;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class BoardPresentation : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform gridRoot;
        
        private int totalRows;
        private int totalColumns;
        
        public void DrawBoard(TileData[,] tiles)
        {
            gridRoot.gameObject.DestroyChildObjects();
            totalRows = tiles.GetLength(0);
            totalColumns = tiles.GetLength(1);
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalColumns; col++)
                {
                    DrawTile(tiles[row, col]);
                }
            }
        }

        public Tile DrawTile(TileData tileData)
        {
            GameObject tileGameObject = tileData.Type switch
            {
                TileType.Empty => Instantiate(emptyTilePrefab),
                TileType.Ground => TileFactory.GetRandomTile(true),
                TileType.Building => TileFactory.GetRandomTile(true),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (tileGameObject != null)
            {
                tileGameObject.transform.SetParent(gridRoot);
                tileGameObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(tileData.Position, totalRows, totalColumns);
                tileGameObject.name = $"Tile {tileData.Type} - [{tileData.Position.x}, {tileData.Position.y}]";

                if (tileGameObject.TryGetComponent(out Tile tile))
                {
                    tile.Init(tileData);
                    return tile;
                }
            }

            return null;
        }
    }
}