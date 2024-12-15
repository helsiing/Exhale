using UnityEngine;

namespace Exhale.Scripts.Board
{
    public class TileSimulation : MonoBehaviour, ITileBoardPositionProvider
    {
        private Tile tile;
        public Vector2 BoardPosition => tile.Position;

        public void Init(Tile tile)
        {
            this.tile = tile;
        }
    }

    public interface ITileBoardPositionProvider
    {
        public Vector2 BoardPosition { get; }
    }
}