using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Data
{
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
