using System;
using BrunoMikoski.ScriptableObjectCollections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable] 
    public struct PieceTemplateData : IComponentData
    {
        public int PieceId;
    } 
    
    [Serializable] 
    public struct BoardPosition : IComponentData
    {
        public int2 PositionIndex;
    } 
    
    public class HexPieceTemplate : ScriptableObjectCollectionItem
    {
        [SerializeField, SerializeReference]
        private PieceTrait[] Traits;
        
        public int GetId()
        {
            return GUID.GetHashCode();
        }
        
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