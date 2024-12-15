using System;
using Exhale.Scripts.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Scripts.Board
{
    [RequireComponent(typeof(BoardSimulation))]
    [RequireComponent(typeof(BoardPresentation))]
    public class Board : MonoBehaviour
    {
        [SerializeField] private BoardConfig boardConfig;
        
        private BoardSimulation boardSimulation;
        private BoardPresentation boardPresentation;
        private readonly BoardLogic boardLogic = new();

        private void Awake()
        {
            TryGetComponent(out boardSimulation);
            TryGetComponent(out boardPresentation);
            
            boardSimulation.OnPlaceTileEvent += OnPlaceTile;
        }

        private void Start() 
        {
            Vector2 centerCell = BoardHelper.GetBoardCenter(boardConfig.Width, boardConfig.Height);
         
            boardLogic.InitBoard(boardConfig.Width, boardConfig.Height);
            
            boardLogic.PlaceTile(centerCell, TileType.Ground);
            boardLogic.PlaceTile((int)centerCell.x + 1, (int)centerCell.y + 1, TileType.Building);
            
            boardPresentation.DrawBoard(boardLogic.Tiles);
        }
        
        void OnPlaceTile(Vector2 position)
        {
            var tile = boardLogic.PlaceTile(position, TileType.Building);
            Assert.IsNotNull(tile, "tile != null");
            
            boardPresentation.DrawTile(tile);
        }

        private void OnDestroy()
        {
            boardSimulation.OnPlaceTileEvent -= OnPlaceTile;
        }
    }
}