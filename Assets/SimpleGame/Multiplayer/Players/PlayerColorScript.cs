using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer.Players
{
    /// <summary>
    /// Component for on the player to assign a unique color to each player
    /// </summary>
    public class PlayerColorScript : NetworkBehaviour
    {
        [SyncVar]
        private Color playerColor;

        public event Action<Color> OnPlayerColorChanged;


        public void Start()
        {
            if (isServer)
                setColorServerside(MultiplayerGameStateManager.Instance.GetFreePlayerColor());
            else
                onColorChanged(playerColor);

            OnPlayerColorChanged += c => GetComponent<PlayerModelScript>().SetColorOfPlayerModel(c);

            GetComponent<PlayerModelScript>().SetColorOfPlayerModel(playerColor);
        }

        private void onColorChanged(Color color)
        {
            if (OnPlayerColorChanged != null)
                OnPlayerColorChanged(color);
        }


        public Color GetPlayerColor()
        {
            return playerColor;
        }

        public void setColorServerside(Color color)
        {
            playerColor = color;
            RpcSetPlayerColor(color);
        }

        [ClientRpc]
        public void RpcSetPlayerColor(Color color)
        {
            playerColor = color;
            onColorChanged(color);
        }
    }
}