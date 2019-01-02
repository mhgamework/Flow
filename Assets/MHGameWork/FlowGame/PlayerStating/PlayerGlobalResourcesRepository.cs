using System.Collections.Generic;
using System.Linq;
using Assets.MHGameWork.FlowGame.Domain;
using Assets.Reusable;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.PlayerStating
{
    /// <summary>
    /// Stores the global resources of the player
    /// </summary>
    public class PlayerGlobalResourcesRepository
    {
        private Dictionary<ResourceType, int> amounts = new Dictionary<ResourceType, int>();
        private Dictionary<ResourceType, int> maxes = new Dictionary<ResourceType, int>();

        public void SetMaxResourceAmount(ResourceType type, int max)
        {
            maxes[type] = max;
        }

        public int GetMaxResourceAmount(ResourceType type)
        {
            return maxes.GetOrDefault(type, 0);
        }
        /// <summary>
        /// Returns the amount of resources actually stored
        /// </summary>
        public int RequestStoreResources(ResourceType type, int amount)
        {
            return changeResourceAmount(type, amount);
        }

        /// <summary>
        /// Returns num actually taken
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int RequestTakeResources(ResourceType type, int amount)
        {
            return -changeResourceAmount(type, -amount);
        }

        private int changeResourceAmount(ResourceType type, int amount)
        {
            var max = GetMaxResourceAmount(type);
            var val = GetResourceAmount(type);
            amount = Mathf.Min(amount, max - val); // dont store more than possible
            amount = Mathf.Max(amount, -val); // dont take more than available
            var newVal = val + amount;

            amounts[type] = newVal;

            return newVal - val;
        }

        public int GetResourceAmount(ResourceType type)
        {
            return amounts.GetOrDefault(type, 0);
        }

        public IEnumerable<ResourceType> GetStoredResourceTypes()
        {
            return amounts.Where(f => f.Value != 0).Select(f => f.Key);
        }
    }
}