using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Component for on player that implements MP player lives
    /// </summary>
    public class PlayerLivesScript : NetworkBehaviour
    {
        [SyncVar] public int Lives;

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
    }
}