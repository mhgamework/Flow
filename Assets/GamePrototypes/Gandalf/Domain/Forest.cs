using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class Forest:ICellEntity
    {
        public int MaxWood = 4;
        public float GrowInterval;
        public Cell Position { get; private set; }
        public Forest(Cell position)
        {
            this.Position = position;
            
            position.Register(this);
        }

        public IEnumerable<YieldInstruction> Simulate()
        {
            for (;;)
            {
                yield return new WaitForSeconds(GrowInterval);
                if (Position.Items.Get(ItemType.Wood) >= MaxWood) continue;
                Position.Items.Add(ItemType.Wood, 1);
            }
        }

        public string GetCellInfo()
        {
            return "Forest ";
        }

        public IDictionary<string, string> Values { get { return new Dictionary<string,string>(); } }
    }
};