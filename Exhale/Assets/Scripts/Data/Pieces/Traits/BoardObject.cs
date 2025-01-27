using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Exhale.Scripts.Data
{

    /// <summary>
    ///     Trait that indicates the board object that should be instantiated for a piece.
    /// </summary>
    [Serializable]
    public class BoardObject : PieceTrait
    {
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;

        public override bool ValidateConfig()
        {
            return prefab != null;
        }
    }
}