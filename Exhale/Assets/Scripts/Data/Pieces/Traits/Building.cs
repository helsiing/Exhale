using System;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public class Building : PieceTrait
    {
        public override bool ValidateConfig()
        {
            return true;
        }
    }
}