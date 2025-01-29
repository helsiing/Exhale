using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public struct BoardDataComponent : IComponentData
    {
        public int Width;
        public int Height;
        public int2 StartPosition;
        [NonSerialized] public Entity EmptyTTilePrefabEntity;
    }
    
    public enum BoardStartType
    {
        Random,
        Center,
        AtPosition
    }
    
    public struct BoardInitializedEvent : IComponentData
    {
        public bool IsInitialized;
        public int2 StartPosition;
    }

    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Exhale/BoardConfig", order = 0)]
    public class BoardConfig : ScriptableObject
    {
        [InlineProperty, HideLabel, SerializeField]
        private BoardDataComponent data;
        public BoardDataComponent Data => data;
        
        [SerializeField] private BoardStartType startType;
        public BoardStartType StartType => startType;
        
        [SerializeField] private GameObject emptyTilePrefab;
        public GameObject EmptyTilePrefab => emptyTilePrefab;
    }
}