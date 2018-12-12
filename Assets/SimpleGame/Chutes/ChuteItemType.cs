using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    [Serializable]
    public class ChuteItemType
    {
        public string Id;
        public Color Color;
        public bool IsFuel;
        public ChuteItemType Smelted;

        public static List<ChuteItemType> AllTypes = new List<ChuteItemType>();

        public static ChuteItemType Red;
        public static ChuteItemType Green;
        public static ChuteItemType Blue;
        private static ChuteItemType GreenSmelted;
        private static ChuteItemType BlueSmelted;

        static ChuteItemType()
        {
            Red = create("red", Color.red).Fuel().Build();
            Green = create("green", Color.green).Build();
            Blue = create("blue", Color.blue).Build();

            GreenSmelted = create("green-smelted", new Color(0.5f,1,0.5f)).Smelted(Green).Build();
            BlueSmelted = create("blue-smelted", new Color(0.5f,0.5f,1)).Smelted(Blue).Build();
        }

        static Builder create(string uniqueId, Color c)
        {
            return new Builder(uniqueId, c);
        }

        public static ChuteItemType GetById(string id)
        {
            var ret = AllTypes.FirstOrDefault(t => t.Id == id);
            if (ret == null) throw new Exception("Item type with id not found " + id);
            return ret;
        }

        class Builder
        {
            private ChuteItemType type;
            public Builder(string uniqueId, Color c)
            {
                type = new ChuteItemType()
                {
                    Color = c,
                    Id = uniqueId
                };
                if (AllTypes.Any(f => f.Id == type.Id))
                    throw new Exception("Already item with this type");
                AllTypes.Add(type);
            }

            public Builder Fuel()
            {
                type.IsFuel = true;
                return this;
            }

            public Builder Smelted(ChuteItemType smeltedFrom)
            {
                smeltedFrom.Smelted = type;
                return this;
            }

            public ChuteItemType Build()
            {

                return type;
            }
        }
    }
}