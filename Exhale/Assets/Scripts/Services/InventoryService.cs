using System.Collections.Generic;
using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Gameplay;

namespace Exhale.Scripts.Services
{
    public interface IInventoryService : IService
    {
        public void AddYield(YieldTemplate yieldTemplate, int amount);
    }
    
    public class InventoryService : IInventoryService
    {
        private readonly Dictionary<YieldTemplate, int> yieldInventory = new();

        public void AddYield(YieldTemplate yieldTemplate, int amount)
        {
            if (!yieldInventory.TryAdd(yieldTemplate, amount))
            {
                yieldInventory[yieldTemplate] += amount;
            }
        }
        public void Dispose()
        {
        }
    }
}