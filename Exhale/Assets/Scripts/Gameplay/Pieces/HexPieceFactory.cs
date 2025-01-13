using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public static class HexPieceFactory
    {
        public static HexPieceTemplate GetRandomTemplate<T> () where T : PieceTrait
        {
            return HexPieceTemplateCollection.GetRandomTemplate<T>();
        }
        
    }
}