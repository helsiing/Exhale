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
        [NonSerialized] public int EmptyPieceId;
        [NonSerialized] public Entity EmptyPiecePrefabEntity;
    }

    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Exhale/BoardConfig", order = 0)]
    public class BoardConfig : ScriptableObject
    {
        [InlineProperty] [HideLabel] [SerializeField]
        private BoardDataComponent data;

        [SerializeField] private HexPieceTemplate emptyPieceTemplate;
        public BoardDataComponent Data => data;
        public HexPieceTemplate EmptyPieceTemplate => emptyPieceTemplate;

        public int Width => data.Width;
        public int Height => data.Height;
    }
}