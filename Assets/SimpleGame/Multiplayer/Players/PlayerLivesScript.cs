using Assets.SimpleGame.Multiplayer.Players;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Component for on player that implements MP player lives
    /// </summary>
    public class PlayerLivesScript : NetworkBehaviour
    {
        private Vector3 spawnLocation;

        [SyncVar] public int Lives;

        public void Start()
        {
            var model = GetComponent<PlayerModelScript>();
            model.initialize();

            var movement = GetComponent<PlayerMovementScript>();
            movement.initialize();

            ScoreManager.Instance.RegisterPlayer(GetComponent<PlayerLivesScript>());
            spawnLocation = transform.position;

        }

        private void OnDisable()
        {
            ScoreManager.Instance.UnRegisterPlayer(GetComponent<PlayerLivesScript>());

        }

        public void Update()
        {
            checkOutOfMap();
        }


        public void ResetLives(int startLives)
        {
            if (!isServer) return;
            RpcSetLives(startLives);
        }

        [ClientRpc]
        private void RpcSetLives(int lives)
        {
            Lives = lives;
            Debug.Log("lives " + lives);
        }

        [Command]
        private void CmdTakeLife()
        {
            TakeLife();
        }

        public void TakeLife()
        {
            if (isServer)
            {
                if (Lives == 1)
                {
                    ScoreManager.Instance.OnPlayerDeath(this);
                }
                else
                {
                    RpcSetLives(Lives - 1);
                }
            }
            else
            {
                // Not the server, so send to server
                CmdTakeLife();
            }

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
            // Acky!
            GetComponent<PlayerPushScript>().ResetOnPlayerDeath();

        }
    }
}