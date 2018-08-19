using Assets.SimpleGame.Multiplayer.Players;
using Assets.SimpleGame.Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame
{
    /// <summary>
    /// Responsible for understanding the structure of the player prefab.
    /// Used to provide access to components of the local player
    /// Seems somewhat of a wiring component
    /// </summary>
    public class LocalPlayerScript : NetworkBehaviour
    {
        private static LocalPlayerScript staticInstance = null;

        void Start()
        {
        }

        public override void OnStartLocalPlayer()
        {
            staticInstance = this;

            SimpleGameSystemScript.Instance.OnLocalPlayerCreated(this);
            base.OnStartLocalPlayer();
        }

        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            if (staticInstance == this)
                staticInstance = null;

        }

        public Camera GetCamera()
        {
            return GetComponentInChildren<Camera>();
        }

        public FirstPersonController GetFirstPersonController()
        {
            return GetComponentInChildren<FirstPersonController>();
        }

        public PlayerScript GetPlayer()
        {
            return GetComponentInChildren<PlayerScript>();
        }

        public void Initialize(SimpleGameHUDScript hud)
        {
            // Fast initialize this!, so camera exists!

            GetComponent<PlayerModelScript>().initialize();

            //TODO this is shitty and should be inverted. Model should not depend on UI directly
            GetPlayer().Initialize(hud.GetHotbarInventory());
        }

        public static LocalPlayerScript Instance { get { return staticInstance; } }
    }
}