using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public struct BoardData : IComponentData
    {
        public int Width;
        public int Height;
        public Entity HexTilePrefab;
    }
    
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Exhale/BoardConfig", order = 0)]
    public class BoardConfig : ScriptableObject
    {
        [InlineProperty, HideLabel, SerializeField]
        private BoardData data;
        public BoardData Data => data;
        
        [SerializeField]
        private GameObject hexTilePiecePrefab;
        public GameObject HexTilePiecePrefab => hexTilePiecePrefab;
        
        public int Width => data.Width;
        public int Height => data.Height;
    }
}