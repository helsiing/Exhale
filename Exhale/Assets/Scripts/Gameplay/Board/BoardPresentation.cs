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
        
        public void DrawBoard(TileData[,] tiles)
        {
            gridRoot.gameObject.DestroyChildObjects();
            for (int row = 0; row < tiles.GetLength(0); row++)
            {
                for (int col = 0; col < tiles.GetLength(1); col++)
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
                tileGameObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(tileData.Position);

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