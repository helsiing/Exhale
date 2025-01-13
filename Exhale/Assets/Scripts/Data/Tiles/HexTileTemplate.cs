using System;
using BrunoMikoski.ScriptableObjectCollections;
using Exhale.Scripts.Gameplay;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public class HexTileElements
    {
        public HexTileElement[] Sides = new HexTileElement[6];
    }
    
    public abstract class HexTileTemplate : ScriptableObjectCollectionItem
    {
        [SerializeField]
        private GameObject boardPrefab;
        public GameObject BoardPrefab => boardPrefab;

        public HexTileElements Elements;
    }
}
