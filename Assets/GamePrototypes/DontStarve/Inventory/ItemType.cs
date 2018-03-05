using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePrototypes.DontStarve;

namespace Assets.DontStarve.Inventory
{
    public class ItemType
    {
        public String Name;

        public ItemType(string name)
        {
            Name = name;
        }



        public bool IsEdible { get; private set; }

        private float EatFood;
        private float EatHealth;
        public void SetEdible(float food, float health)
        {
            IsEdible = true;
            EatFood = food;
            EatHealth = health;
        }
        public void Eat(PlayerScript player)
        {
            player.takeDamage(-EatHealth);
            player.gainFood(EatFood);
        }


        public bool IsCookable { get; private set; }
        public string CookedItemType { get; private set; }

        public void SetCookable(string cookedItemType)
        {
            IsCookable = true;
            CookedItemType = cookedItemType;
        }


        public bool IsFuel { get; private set; }
        public float FuelAmount { get; private set; }
        public void SetFuel(int amount)
        {
            IsFuel = true;
            FuelAmount = amount;
        }
    }
}
