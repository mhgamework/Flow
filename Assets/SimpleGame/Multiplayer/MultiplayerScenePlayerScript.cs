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
    /// </summary>
    public class MultiplayerScenePlayerScript : NetworkBehaviour
    {
        //[SyncVar] public int Score = 0;

        private Vector3 spawnLocation;

   

        private PlayerGrenadeScript playerGrenadeScript;
        private PlayerPushScript playerPushScript;

        public void Start()
        {
            var model = GetComponent<PlayerModelScript>();
            model.initialize();

            var movement = GetComponent<PlayerMovementScript>();
            movement.initialize();

            ScoreManager.Instance.RegisterPlayer(GetComponent<PlayerLivesScript>());

            
            playerGrenadeScript = GetComponent<PlayerGrenadeScript>();
            playerPushScript = GetComponent<PlayerPushScript>();
            playerPushScript.initialize(model.GetHead());

            GetComponent<PlayerColorScript>().OnPlayerColorChanged += c => model.SetColorOfPlayerModel(c);

            Debug.Log("Start networked player");
//            if (!isLocalPlayer)
//            {
//                disableLocalPlayerOnlyComponents();
//            }


            spawnLocation = transform.position;
        }

       
      

        public override void OnStartLocalPlayer()
        {
            Debug.Log("OnStartLocalPlayer");
            var movement = GetComponent<PlayerMovementScript>();
            movement.StartLocalPlayerCameraAndInput();

            //enableLocalPlayerOnlyComponents();


        }

      

        public override void OnNetworkDestroy()
        {
            ScoreManager.Instance.UnRegisterPlayer(GetComponent<PlayerLivesScript>());
            base.OnNetworkDestroy();
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


            checkOutOfMap();
        }

        private void checkOutOfMap()
        {
            if (transform.position.y < -100)
            {
                OnFallOfWorld();
            }
        }

       

        public void OnFallOfWorld()
        {
            if (!isLocalPlayer) return;
            GetComponent<PlayerLivesScript>().TakeLife();
            transform.position = spawnLocation;
            playerPushScript.ResetOnPlayerDeath();

        }


     
    }
}