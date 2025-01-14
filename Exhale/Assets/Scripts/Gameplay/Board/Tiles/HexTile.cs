using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface IHexTile
    {
        public bool IsOccupied { get; }
        public void Init(HexTileData hexTileData);
        public void Show();
    }
    
    [RequireComponent(typeof(HexTileSimulation))]
    [RequireComponent(typeof(HexTilePresentation))]
    public class HexTile : MonoBehaviour, IHexTile, IBoardPositionProvider
    {
        private IHexTileSimulation tileSimulation;
        private IHexTilePresentation tilePresentation;
        
        private HexTileData hexTileData;
        public Vector2 PositionIndex => hexTileData.PositionIndex;

        public bool IsOccupied => hexTileData.IsOccupied;

        public void Init(HexTileData hexTileData)
        {
            this.hexTileData = hexTileData;
            tileSimulation = GetComponent<IHexTileSimulation>();
            tilePresentation = GetComponent<IHexTilePresentation>();
        }

        public void Show()
        {
            tilePresentation.Show();
        }
    }
}