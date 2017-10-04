using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class Forest
    {
        public float GrowInterval;
        public ItemCollection Items { get; private set; }

        public Forest()
        {
            Items = new ItemCollection();
        }

        public IEnumerable<YieldInstruction> Simulate()
        {
            for (;;)
            {
                yield return new WaitForSeconds(GrowInterval);
                Items.Add(ItemType.Wood, 1);
            }
        }
    }
};