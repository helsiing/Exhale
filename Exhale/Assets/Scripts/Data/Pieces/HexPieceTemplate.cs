using System;
using BrunoMikoski.ScriptableObjectCollections;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public struct HexPieceTemplateData : IComponentData
    {
        [NonSerialized] public Entity PiecePrefabEntity;
    }
    
    public class HexPieceTemplate : ScriptableObjectCollectionItem
    {
        [InlineProperty, HideLabel, SerializeField]
        private HexPieceTemplateData data;
        public HexPieceTemplateData Data => data;
        
        [SerializeField] private GameObject piecePrefab;
        public GameObject PiecePrefab => piecePrefab;
    }
}
