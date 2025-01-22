using System;
using BrunoMikoski.ScriptableObjectCollections;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public struct PieceTemplate : IComponentData
    {
        public int PieceId;
        [NonSerialized] public Entity PiecePrefabEntity;
    }

    public class HexPieceTemplate : ScriptableObjectCollectionItem
    {
        [InlineProperty] [HideLabel] [SerializeField]
        private PieceTemplate data;

        [SerializeField] private GameObject piecePrefab;
        public PieceTemplate Data => data;
        public GameObject PiecePrefab => piecePrefab;
    }
}