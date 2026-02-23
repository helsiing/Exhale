using System.Collections.Generic;
using BrunoMikoski.ScriptableObjectCollections;
using LBG;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    #region ECS

    #endregion
    
    public class HexPieceTemplate : ScriptableObjectCollectionItem
    {
        [SerializeField, SerializeReference, SubclassSelector]
        private List<PieceTrait> traits = new();
        public List<PieceTrait> Traits => traits;
        
        public int GetId()
        {
            return GUID.GetHashCode();
        }
        
        public GameObject GetPrefab()
        {
            if (TryGetTrait(out BoardObject boardObject))
            {
                return boardObject.Prefab;
            }

            return null;
        }
        
        public bool TryGetTrait<T>(out T trait) where T : PieceTrait
        {
            int index = -1;

            if (traits == null)
            {
                trait = null;
                return false;
            }

            for(int i = 0; i < traits.Count; i++)
            {
                if(traits[i] == null)
                {
                    trait = null;
                    return false;
                }
                    
                if (traits[i].GetType() == typeof(T))
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

            trait = traits[index] as T;
            return true;
        }
        
        public bool HasTrait<T>() where T : PieceTrait
        {
            return TryGetTrait(out T _);
        }

        public bool ValidateConfig()
        {
            foreach (var trait in traits)
            {
                if (!trait.ValidateConfig(this))
                {
                    return false;
                }
            }

            return true;
        }
        
        
        
    }
}