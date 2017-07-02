using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Homm
{
    public class PlayerState : Singleton<PlayerState>
    {
        public Wizard Wizard;
        public List<ResourceAmount> Resources = new List<ResourceAmount>();

        public void Start()
        {
            foreach (var val in Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>())
            {
                if (Resources.All(f => f.Type != val))
                    Resources.Add(new ResourceAmount { Type = val, Amount = 0 });
            }
        }

        public void ChangeResourceAmount(ResourceType type, int change)
        {
            Resources.First(f => f.Type == type).Amount += change;
        }
        public int GetResourceAmount(ResourceType type)
        {
            return Resources.First(f => f.Type == type).Amount;
        }

        [Serializable]
        public class ResourceAmount
        {
            public ResourceType Type;
            public int Amount;
        }
    }
}