using Assets.SimpleGame.Scripts.UI;
using Assets.SimpleGame.Wards;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.Scripts
{
    public class PlayerScript : Singleton<PlayerScript>
    {
        public InventoryScript HotbarInventory;
        public float Health = 80;
        public float MaxHealth = 100;

        public bool AirSpellCasting = false;

        private Vector3 playerStartPos;

        public EntityScript Entity { get; set; }
        private FirstPersonController firstPersonController;

        public float BaseMovementSpeed;


        public void Initialize(InventoryScript hotbarInventory)
        {
            //TODO: THe data for the players inventory should not be in the UI!
            this.HotbarInventory = hotbarInventory;
        }

        public void Start()
        {
            playerStartPos = transform.position;
            Entity = GetComponent<EntityScript>();
            firstPersonController = GetComponent<FirstPersonController>();
        }

        public void Update()
        {
            firstPersonController.SpeedMultiplier = Entity.SpeedMultiplier;
        }



        public Vector3 GetPlayerPosition()
        {
            return transform.position;
        }

        public Transform GetCameraTransform()
        {
            return GetComponentInChildren<Camera>().transform;
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

        public void ToggleAirSpellCasting()
        {
            AirSpellCasting = !AirSpellCasting;
        }
    }
}