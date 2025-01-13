using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public interface IBoardPositionProvider
    {
        public Vector2 PositionIndex { get; }
    }
}