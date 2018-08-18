using System;
using UnityEngine;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Group script for the group prefab for the multiplayer system
    /// </summary>
    public class MultiplayerSystemScript:MonoBehaviour
    {
        public void SetPlayerPrefab(GameObject PlayerPrefab)
        {
            NetworkManager.playerPrefab = PlayerPrefab;
        }

        public CustomNetworkManagerScript NetworkManager
        {
            get { return GetComponentInChildren<CustomNetworkManagerScript>(); }
        }

        public void AutostartHostIfEditor()
        {
            if (Application.isEditor)
                NetworkManager.StartHost();

        }
    }
}