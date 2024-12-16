using System;
using Exhale.Scripts.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Exhale.Scripts.Gameplay
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
            boardPresentation.DrawBoard(boardLogic.Tiles);
            
            var tile = PlaceTile(centerCell, TileType.Building);
            
        }

        private Tile PlaceTile(Vector2 position, TileType type)
        {
            var tile = boardLogic.PlaceTile(position, type);
            Assert.IsNotNull(tile, "tile != null");
            return boardPresentation.DrawTile(tile);
        }
        
        void OnPlaceTile(Vector2 position)
        {
            PlaceTile(position, TileType.Building);
        }

        private void OnDestroy()
        {
            boardSimulation.OnPlaceTileEvent -= OnPlaceTile;
        }
    }
}