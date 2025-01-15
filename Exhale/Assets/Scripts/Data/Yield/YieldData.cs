using System;
using Exhale.Scripts.Gameplay;
using UnityEngine;

namespace Data.Yield
{
    [Serializable]
    public class YieldData
    {
        [SerializeField] private YieldTemplate yieldTemplate;
        public YieldTemplate YieldTemplate => yieldTemplate;
        
        [SerializeField] private int amount;
        public int Amount => amount;
    }
}