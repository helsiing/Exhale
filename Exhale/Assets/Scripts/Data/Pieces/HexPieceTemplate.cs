using System;
using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    /// <summary>
    /// Base class that defines a trait ("characteristic") of a piece.
    /// </summary>
    [Serializable]
    public abstract class PieceTrait
    {
        public abstract bool ValidateConfig();
    }
    
    /// <summary>
    /// Trait that indicates the board object that should be instantiated for a piece.
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
    
    public class HexPieceTemplate : ScriptableObjectCollectionItem
    {
        [SerializeReference]
        public PieceTrait[] Traits;
        
        public bool TryGetTrait<T>(out T trait) where T : PieceTrait
        {
            int index = -1;
            for(int i = 0; i < Traits.Length; i++)
            {
                if (Traits[i].GetType() == typeof(T))
                {
                    index = i;
                    break;
                }
            }
            if (index < 0)
            {
                trait = null;
                return false;
            }

            trait = Traits[index] as T;
            return true;
        }
        
        public bool HasTrait<T>() where T : PieceTrait
        {
            return TryGetTrait(out T _);
        }

        public bool ValidateConfig()
        {
            foreach (PieceTrait trait in Traits)
            {
                if (!trait.ValidateConfig())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
