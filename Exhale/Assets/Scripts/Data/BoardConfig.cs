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
        // Baked from BoardConfig.PrePlacedGroundCount. Number of randomly scattered ground
        // tiles placed at boot. 0 = none.
        [NonSerialized] public int PrePlacedGroundCount;
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

        [Tooltip("How many ground tiles to scatter randomly across the board at boot, each " +
                 "assigned a random Ground-trait piece. They need not be adjacent; blank " +
                 "spaces are expected. Clamped to the number of tiles on the board. 0 = none.")]
        [SerializeField] private int prePlacedGroundCount = 36;
        public int PrePlacedGroundCount => prePlacedGroundCount;
    }
}