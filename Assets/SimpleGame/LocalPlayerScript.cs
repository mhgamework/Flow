using Assets.SimpleGame.Scripts;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame
{
    /// <summary>
    /// Responsible for understanding the structure of the player prefab.
    /// Used to provide access to components of the local player
    /// Seems somewhat of a wiring component
    /// </summary>
    public class LocalPlayerScript : Singleton<LocalPlayerScript>
    {
        public Camera GetCamera()
        {
            return GetComponentInChildren<Camera>();
        }

        public FirstPersonController GetFirstPersonController()
        {
            return GetComponentInChildren<FirstPersonController>();
        }

        private PlayerScript GetPlayer()
        {
            return GetComponentInChildren<PlayerScript>();
        }

        public void Initialize(SimpleGameHUDScript hud)
        {
            //TODO this is shitty and should be inverted. Model should not depend on UI directly
            GetPlayer().Initialize(hud.GetHotbarInventory());
        }
    }
}