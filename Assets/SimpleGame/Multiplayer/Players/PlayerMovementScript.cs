using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.Multiplayer.Players
{
    /// <summary>
    /// Component for player that implements MP player movement.
    /// Requires a charactercontroller and fpscontrolelr to be present, due to "limitations"
    /// in unity this has to be done manually
    /// </summary>
    public class PlayerMovementScript : NetworkBehaviour
    {
//        [SerializeField] private Transform PlayerControllerCamera;

        private bool init = false;
        private Transform playerControllerCamera;

        public void Start()
        {
            if(GetComponent<FirstPersonController>() == null) throw new Exception("Needs FPSControlelr");
            if(GetComponent<CharacterController>() == null) throw new Exception("Needs Character controller");

            initialize();
        }
        public void initialize()
        {
            if (init) return;
            // Danger, hope this doesnt break shit
            var playerModelScript = GetComponent<PlayerModelScript>();
            playerModelScript.initialize();// Initialize this, idempotent
            //playerControllerCamera = Instantiate(PlayerControllerCamera, playerModelScript.GetHead());

            //TODO: this is a hacky solution
            GetComponent<NetworkTransformChild>().target = playerModelScript.GetHead();

            if (playerModelScript.GetHead().GetComponent<Camera>() == null)
                throw new Exception("PlayerModel should have a camera attached to the head");
            if (playerModelScript.GetHead().GetComponent<AudioListener>() == null)
                throw new Exception("PlayerModel should have an audiolistener attached to the head");


            if (isLocalPlayer)
                enableLocalPlayerOnlyComponents();
            else
                disableLocalPlayerOnlyComponents();



            init = true;
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