using Assets.SimpleGame.Scripts.UI;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class PlayerScript : Singleton<PlayerScript>
    {
        public InventoryScript HotbarInventory;
        public float Health = 80;
        public float MaxHealth = 100;

        private Vector3 playerStartPos;


        public void Start()
        {
            playerStartPos = transform.position;

        }


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

        public void TakeDamage(float damage)
        {
            Health = Mathf.Max(0, Health - damage);
            HitDamageOverlayScript.Instance.OnHit();

            if (Health == 0)
            {
                Respawn();
            }
        }

        public void Respawn()
        {
            transform.position = playerStartPos;
            Health = 100;
        }
    }
}