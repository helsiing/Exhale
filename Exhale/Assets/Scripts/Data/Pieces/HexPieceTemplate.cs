using System;
using BrunoMikoski.ScriptableObjectCollections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exhale.Scripts.Data
{
    #region ECS
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
    #endregion
    
    public class HexPieceTemplate : ScriptableObjectCollectionItem
    {
        [FormerlySerializedAs("Traits")] [SerializeField, SerializeReference]
        private PieceTrait[] traits;
        public PieceTrait[] Traits => traits;
        
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

            Debug.LogError($"The piece {name} does not have a BoardObject trait");
            return null;
        }
        
        public bool TryGetTrait<T>(out T trait) where T : PieceTrait
        {
            int index = -1;
            for(int i = 0; i < traits.Length; i++)
            {
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
            foreach (PieceTrait trait in traits)
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