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
        
        public void DrawBoard(HexTileData[,] tiles)
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

        public HexTile DrawTile(HexTileData hexTileData)
        {
            /*GameObject tileGameObject = hexTileData.Type switch
            {
                HexTileType.Empty => Instantiate(emptyTilePrefab),
                HexTileType.Ground => HexTileFactory.GetRandomTile(true),
                HexTileType.Building => HexTileFactory.GetRandomTile(true),
                _ => throw new ArgumentOutOfRangeException()
            };*/

            GameObject tileGameObject = HexTileFactory.GetRandomTile(true);

            if (tileGameObject != null)
            {
                tileGameObject.transform.SetParent(gridRoot);
                tileGameObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(hexTileData.Position, totalRows, totalColumns);
                tileGameObject.name = $"HexTile [{hexTileData.Position.x}, {hexTileData.Position.y}]";

                if (tileGameObject.TryGetComponent(out HexTile tile))
                {
                    tile.Init(hexTileData);
                    return tile;
                }
            }

            return null;
        }
    }
}