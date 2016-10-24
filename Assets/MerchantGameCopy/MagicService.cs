using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class MagicService
    {
        public static MagicService Get
        {
            get
            {
                return new MagicService();
            }
        }

        private List<MagicType> types = new List<MagicType>();

        private MagicService()
        {
            types.Add(new MagicType() { Identifier = "air", Color = Color.white });
            types.Add(new MagicType() { Identifier = "fire", Color = Color.red });
            types.Add(new MagicType() { Identifier = "earth", Color = Color.green });
            types.Add(new MagicType() { Identifier = "water", Color = Color.blue });
        }

        public Color GetColor(string magicType)
        {
            var type = types.FirstOrDefault(t => t.Identifier == magicType);

            if (type != null) return type.Color;

            return Color.gray;
        }


        public class MagicType
        {
            public Color Color;
            public string Identifier;
        }
    }
}