using System;
using System.Collections.Generic;
using Data.Yield;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public class Yield : PieceTrait
    {
        [SerializeField] private List<YieldData> yields = new();
        public List<YieldData> Yields => yields;

        public override bool ValidateConfig()
        {
            return true;
        }
    }

    [Serializable]
    public class Ground : PieceTrait
    {
        public override bool ValidateConfig()
        {
            return true;
        }
    }
}