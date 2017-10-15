using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Gameplay
{
    public class InventoryScript : MonoBehaviour
    {
        [SerializeField]
        private List<Entry> Entries = new List<Entry>();

        public float Max = 10f;


        public void Add(string type, float amount)
        {
            if (amount < 0) throw new Exception();
            Change(type, amount);
        }
        private float Change(string type, float amount)
        {
            var entry = GetOrCreateEntry(type);
            var ori = entry.Amount;
            entry.Amount = Mathf.Clamp(entry.Amount + amount, 0, Max);
            return entry.Amount - ori;
        }

        private Entry GetOrCreateEntry(string type)
        {
            var ret = Entries.FirstOrDefault(e => e.Type == type);
            if (ret != null) return ret;

            ret = new Entry { Type = type, Amount = 0 };
            Entries.Add(ret);
            return ret;

        }
        public float Get(string type)
        {
            return GetOrCreateEntry(type).Amount;
        }

        public float Take(string type, float amount)
        {
            return -Change(type, -amount);
        }

        public string ToItemString(string separator)
        {
            return Entries.Aggregate("", (acc, el) => acc + el.Type + ": " + el.Amount + separator);
        }

        public override string ToString()
        {
            return "InventoryScript: " + ToItemString("\n");
        }

     

        [Serializable]
        public class Entry
        {
            public string Type;
            public float Amount;
        }
    }
}