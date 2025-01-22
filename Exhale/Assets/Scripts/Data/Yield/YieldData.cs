using System;
using Exhale.Scripts.Gameplay;
using UnityEngine;

namespace Data.Yield
{
    [Serializable]
    public class YieldData
    {
        [SerializeField] private YieldTemplate yieldTemplate;

        [SerializeField] private int amount;
        public YieldTemplate YieldTemplate => yieldTemplate;
        public int Amount => amount;
    }
}