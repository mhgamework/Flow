using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.DontStarve.Crafting
{
    [Serializable]
    public class Recipe
    {
        public GameObject ItemModel;
        public string Name;

        public List<ResourceAmount> ResourceCost;
        public String  Description;

        [Serializable]
        public class ResourceAmount
        {
            public string Resource;
            public int Amount;
        }
    }
}
