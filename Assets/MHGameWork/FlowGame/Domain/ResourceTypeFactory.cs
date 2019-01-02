using System;
using System.Collections.Generic;

namespace Assets.MHGameWork.FlowGame.Domain
{
    public class ResourceTypeFactory
    {
        public static ResourceType MagicCrystals { get; private set; }
        public static ResourceType Rock { get; private set; }
        public static ResourceType Firestone { get; private set; }

        public static List<ResourceType> AllTypes { get; private set; }
        private static Dictionary<string, ResourceType> lookup = new Dictionary<string, ResourceType>();
        static ResourceTypeFactory()
        {
            AllTypes = new List<ResourceType>();
            MagicCrystals = addResource("magicCrystals","Magic Crystals");
            Rock = addResource("rock", "Rock");
            Firestone = addResource("firestone", "Firestone");
        }

        private static ResourceType addResource(string id,string magicCrystals)
        {
            var ret = new ResourceType() {Id=id, Name = magicCrystals };
            AllTypes.Add(ret);
            lookup[ret.Id] = ret;
            return ret;  
        }

        public static ResourceType FindById(string id)
        {
            if (!lookup.ContainsKey(id)) throw new Exception("Cannot find resource type with id " + id);
            return lookup[id];
        }
    }
}