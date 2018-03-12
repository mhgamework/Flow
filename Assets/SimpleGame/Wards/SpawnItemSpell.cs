using System.Linq;
using Assets.SimpleGame.Items;
using Assets.SimpleGame.Scripts;
using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    [CreateAssetMenu(menuName ="Flow/Spells/SpawnItemSpell")]
    public class SpawnItemSpell:AbstractWardSpell
    {
        public bool SelectOnSpawn;
        public MagicParticleSpellItem Item;
        public string ItemResourceType;
        public int Amount;

        public override void Cast(PlayerScript player)
        {
            player.StoreItems(ItemResourceType, Amount);
            if (SelectOnSpawn)
            {
                var stack = player.HotbarInventory.Inventory .FirstOrDefault(f => f.ResourceType == ItemResourceType);
                var index = player.HotbarInventory.Inventory.IndexOf(stack);
                HotbarScript.Instance.SelectedSlot = index;
            }

            player.AirSpellCasting = false;
        }
    }
}