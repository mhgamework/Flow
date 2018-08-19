using System.Linq;
using Assets.SimpleGame.Multiplayer.Players;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// The player script used for the multiplayer demo scene
    /// Only handles input currently
    /// </summary>
    public class MultiplayerScenePlayerScript : NetworkBehaviour
    {
        private PlayerGrenadeScript playerGrenadeScript;
        private PlayerPushScript playerPushScript;

        public void Start()
        {
            playerGrenadeScript = GetComponent<PlayerGrenadeScript>();
            playerPushScript = GetComponent<PlayerPushScript>();

            Debug.Log("Start networked player");

        }

       
      

        public override void OnStartLocalPlayer()
        {
            Debug.Log("OnStartLocalPlayer");

        }

      

     
        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (Input.GetMouseButton(0))
                playerPushScript.DoStartCastPushSpell();
            else
                playerPushScript.DoStopCastPushSpell();


            playerGrenadeScript.SetFireGrenadeDown(Input.GetMouseButton(1));


        }

      


     
    }
}