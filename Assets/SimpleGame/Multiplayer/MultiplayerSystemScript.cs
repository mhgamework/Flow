using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Group script for the group prefab for the multiplayer system
    /// </summary>
    public class MultiplayerSystemScript : MonoBehaviour
    {
        public CustomNetworkManagerScript NetworkManager
        {
            get { return GetComponentInChildren<CustomNetworkManagerScript>(); }
        }

        public bool AutoHostInEditor { get; set; }
        public GameObject PlayerPrefab
        {
            get { return NetworkManager.playerPrefab; }
            set { NetworkManager.playerPrefab = value; }
        }
        public GameObject LobbyPrefab { get; set; }

        private GameObject lobby;

        private bool autoStarted = false;
        private NetworkClient host;


        public void Start()
        {
            showLobby();
            NetworkManager.OnConnectedToGame += hideLobby;
            NetworkManager.OnDisconnectedFromGame += showLobby;
        }

        public void Update()
        {
            if (Application.isEditor && AutoHostInEditor && !autoStarted)
            {
                autoStarted = true;
                host = NetworkManager.StartHost();
                
            }
        }

        private void showLobby()
        {
            if (lobby == null)
            {
                lobby = Instantiate(LobbyPrefab, transform);
            }
            lobby.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //ScoreManager.Instance.Clear();
        }

        private void hideLobby()
        {
            lobby.SetActive(false);
        }
    }
}