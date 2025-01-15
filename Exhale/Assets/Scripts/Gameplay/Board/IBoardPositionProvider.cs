using UnityEngine;

namespace Exhale.Gameplay
{
    public interface IBoardPositionProvider
    {
        public Vector2 PositionIndex { get; }
    }
}