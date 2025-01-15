using System.Collections.Generic;
using Exhale.Scripts.External.ServiceLocators;
using Exhale.Scripts.Gameplay;
using UnityEngine;

namespace Exhale.Services
{
    public interface IInventoryService : IService
    {
        public void AddYield(YieldTemplate yieldTemplate, int amount);
    }
    
    public class InventoryService : IInventoryService
    {
        Dictionary<YieldTemplate, int> yieldsStorage = new();
        
        public void AddYield(YieldTemplate yieldTemplate, int amount)
        {
            if (!yieldsStorage.TryAdd(yieldTemplate, amount))
            {
                yieldsStorage[yieldTemplate] += amount;
            }

            Debug.Log($"Added {amount} of {yieldTemplate.name}");
        }
        
        public void Dispose()
        {
        }
    }
}