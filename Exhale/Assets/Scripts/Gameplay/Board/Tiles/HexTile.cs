using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Gameplay
{
    public interface IHexTile
    {
        public void Init(HexTileData hexTileData);
        public void Show();
        public void Hide();
    }
    
    [RequireComponent(typeof(HexTileSimulation))]
    [RequireComponent(typeof(HexTilePresentation))]
    public class HexTile : MonoBehaviour, IHexTile, IBoardPositionProvider
    {
        private IHexTileSimulation tileSimulation;
        private IHexTilePresentation tilePresentation;
        
        private HexTileData hexTileData;
        public Vector2 PositionIndex => hexTileData.PositionIndex;

        public void Init(HexTileData hexTileData)
        {
            this.hexTileData = hexTileData;
            tileSimulation = GetComponent<IHexTileSimulation>();
            tilePresentation = GetComponent<IHexTilePresentation>();
        }

        public void Hide()
        {
            hexTileData.SetEnabled(false);
            tilePresentation.Hide();
        }

        public void Show()
        {
            if(hexTileData.IsEnabled)
            {
                tilePresentation.Show();
            }
        }
    }
}