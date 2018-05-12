using System;
using Assets.SimpleGame.Scripts;
using UnityEngine;

namespace Assets.SimpleGame
{
    /// <summary>
    /// Responsible for understanding the structure of the HUD canvas prefab.
    /// </summary>
    public class SimpleGameHUDScript : MonoBehaviour
    {
        /// <summary>
        /// TODO: THe data for the players inventory should not be in the UI!
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public InventoryScript GetHotbarInventory()
        {
            return GetComponentInChildren<InventoryScript>();
        }
    }
}