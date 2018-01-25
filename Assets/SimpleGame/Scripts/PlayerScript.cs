using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class PlayerScript : Singleton<PlayerScript>
    {
        public InventoryScript HotbarInventory;
        public float Health = 80;
        public float MaxHealth = 100;

        public Vector3 GetPlayerPosition()
        {
            return transform.position;
        }

        public void StoreItems(string type , int amount)
        {
            HotbarInventory.AddResources(type, amount);
        }

        public int GetNumItems(string type)
        {
            return HotbarInventory.GetResourceCount(type);
        }
        public void TakeItems(string type, int amount)
        {
            HotbarInventory.RemoveResourcse(type, amount);
        }
    }
}