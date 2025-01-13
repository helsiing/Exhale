using UnityEngine;

namespace Exhale.Scripts.Data
{
    public sealed class HexTileBuildingTemplate : HexTileTemplate
    {
        [SerializeField]
        private int maxLevel;
        public int MaxLevel => maxLevel;
    }
}