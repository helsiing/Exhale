using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public struct BoardObjectComponent : IComponentData
    {
        public Entity Prefab;
    }

    /// <summary>
    ///     Trait that indicates the board object that should be instantiated for a piece.
    /// </summary>
    [Serializable]
    public class BoardObject : PieceTrait
    {
        [InlineProperty] [HideLabel] [SerializeField]
        private BoardObjectComponent data;

        [SerializeField] private GameObject prefab;
        public BoardObjectComponent Data => data;
        public GameObject Prefab => prefab;

        public override bool ValidateConfig()
        {
            return prefab != null;
        }
    }
}