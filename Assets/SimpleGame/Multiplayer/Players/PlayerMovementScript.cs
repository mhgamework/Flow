using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.Multiplayer.Players
{
    /// <summary>
    /// Component for player that implements MP player movement.
    /// Uses the PlayerMovementPartPrefab to overlay the necessary components
    /// </summary>
    public class PlayerMovementScript : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementPartPrefabScript PlayerMovementPartPrefab;

        private bool init = false;
        public void initialize()
        {
            // Danger, hope this doesnt break shit
            var temp = Instantiate(PlayerMovementPartPrefab);
            var playerModelScript = GetComponent<PlayerModelScript>();
            playerModelScript.initialize();// Initialize this, idempotent
            temp.ApplyToPlayerGameObject(gameObject,playerModelScript.GetHead());
            Destroy(temp.gameObject);
            disableLocalPlayerOnlyComponents();
            init = true;
        }

        public void StartLocalPlayerCameraAndInput()
        {
            if (!init) initialize();//Dangerous
            enableLocalPlayerOnlyComponents();
        }

        private void disableLocalPlayerOnlyComponents()
        {
            GetComponent<FirstPersonController>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;

        }

        private void enableLocalPlayerOnlyComponents()
        {
            GetComponent<FirstPersonController>().enabled = true;
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
        }


        // Knockback or push force on player. THis is somewhat wierd since this is server-authority
        [ClientRpc]
        public void RpcAddPushVelocity(Vector3 push)
        {
            GetComponent<FirstPersonController>().PushedVelocity += push;
        }

        public void ApplyPushAway(Vector3 pushOrigin, float strength, float strenghtY, float amount)
        {
            if (!isServer) return;

            var dir = (transform.position - pushOrigin).normalized * strength + Vector3.up * strenghtY;
            RpcAddPushVelocity(dir * amount);
        }
    }
}