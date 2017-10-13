using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Gandalf.Domain
{
    public class ItemCollection
    {
        private  Dictionary<ItemType, int> Amounts = new Dictionary<ItemType, int>();
        public bool IsEmpty { get { return Amounts.Values.Sum() == 0; } }

        public void Add(ItemType type, int amount)
        {
            if (amount < 0) throw new Exception();
            Change(type, amount);
        }
        private void Change(ItemType type, int amount)
        {
            if (!Amounts.ContainsKey(type)) Amounts[type] = 0;
            Amounts[type] += amount;
        }

        public int Get(ItemType itemType)
        {
            if (!Amounts.ContainsKey(itemType)) return 0;
            return Amounts[itemType];
        }

        public int Take(ItemType type, int amount)
        {
            var currentAmount = Get(type);
            amount = Math.Min(currentAmount, amount);
            Change(type, -amount);

            return amount;
        }

        public string ToItemString()
        {
            return Amounts.Aggregate("", (acc, el) => acc + el.Key.Name + ": " + el.Value + "\n");
        }

    }
}