using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class ItemType
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }

        private ItemType(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }

        public static readonly ItemType Wood = new ItemType("Wood", Color.black);
    }
}