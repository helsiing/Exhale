using System;
using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private BoardConfig boardConfig;
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform gridRoot;

        private BoardLogic boardLogic = new();
        
        void Start() 
        {
            Vector2 centerCell = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
         
            boardLogic.InitBoard(boardConfig.Width, boardConfig.Height);
            
            boardLogic.PlaceTile((int)centerCell.x, (int)centerCell.y, TileType.Ground);
            boardLogic.PlaceTile((int)centerCell.x + 1, (int)centerCell.y + 1, TileType.Building);
            
            DrawBoard();
        }

        private void DrawBoard()
        {
            Tile[,] tiles = boardLogic.Tiles;
            
            for (int row = 0; row < tiles.GetLength(0); row++)
            {
                for (int col = 0; col < tiles.GetLength(1); col++)
                {
                    GameObject tileGameObject = tiles[row, col].Type switch
                    {
                        TileType.Empty => Instantiate(emptyTilePrefab),
                        TileType.Ground => TileFactory.GetRandomTile(true),
                        TileType.Building => TileFactory.GetRandomTile(true),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (tileGameObject != null)
                    {
                        tileGameObject.transform.SetParent(gridRoot);
                        tileGameObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(row, col);
                    }
                }
            }
        }
    }
}