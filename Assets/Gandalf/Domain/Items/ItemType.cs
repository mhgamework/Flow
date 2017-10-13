using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class ItemType
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }

        private ItemType(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        private static Dictionary<string, ItemType> items = new Dictionary<string, ItemType>();

        static ItemType()
        {
            register(Wood);
            register(Crystals);
            register(Iron);
        }

        private static void register(ItemType type)
        {
            items.Add(type.Name, type);
        }

        public static ItemType Get(string name)
        {
            return items[name];
        }

        public static readonly ItemType Wood = new ItemType("Wood", Color.black);
        public static readonly ItemType Crystals = new ItemType("Crystals", Color.blue);
        public static readonly ItemType Iron = new ItemType("Iron", Color.red);
    }
}