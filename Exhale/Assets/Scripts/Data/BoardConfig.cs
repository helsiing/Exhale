using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public struct BoardDataComponent : IComponentData
    {
        public int Width;
        public int Height;
        [NonSerialized] public Entity EmptyPiecePrefabEntity;
    }

    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Exhale/BoardConfig", order = 0)]
    public class BoardConfig : ScriptableObject
    {
        [InlineProperty, HideLabel, SerializeField]
        private BoardDataComponent data;
        public BoardDataComponent Data => data;
        
        [SerializeField] private GameObject emptyPiecePrefab;
        public GameObject EmptyPiecePrefab => emptyPiecePrefab;

        public int Width => data.Width;
        public int Height => data.Height;
    }
}