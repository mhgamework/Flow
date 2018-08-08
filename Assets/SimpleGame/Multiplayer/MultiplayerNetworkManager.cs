using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace Assets.SimpleGame.Multiplayer
{
    public class MultiplayerNetworkManager : NetworkManager
    {
        // TODO move to a lobby component, or use the NetworkManagers scene mechanism
        public Camera LobbyCamera;

        void Startup()
        {
            showLobby();
        }

        private void showLobby()
        {
            LobbyCamera.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ScoreManager.Instance.Clear();


        }

        private void hideLobby()
        {
            LobbyCamera.enabled = false;
        }



        public override void OnClientConnect(NetworkConnection conn)
        {
            hideLobby();
            Debug.Log("OnClientConnect");
            base.OnClientConnect(conn);
        }
        public override void OnStopClient()
        {
            Debug.Log("OnStopClient");

            showLobby();
            

            base.OnStopClient();
        }

        public override void OnStartClient(NetworkClient client)
        {
            Debug.Log("OnStartClient");

            base.OnStartClient(client);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            Debug.Log("OnClientError");

            base.OnClientError(conn, errorCode);
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            Debug.Log("OnMatchCreate");

            base.OnMatchCreate(success, extendedInfo, matchInfo);
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            Debug.Log("OnServerReady");

            base.OnServerReady(conn);
        }

        public override void OnStartServer()
        {
            Debug.Log("OnStartServer");

            base.OnStartServer();
        }

        public override void OnStopHost()
        {
            Debug.Log("OnStopHost");

            base.OnStopHost();
        }

      

        public override void OnStartHost()
        {
            Debug.Log("OnStartHost");

            base.OnStartHost();

        }

        public override void OnStopServer()
        {
            Debug.Log("OnStopServer");

            base.OnStopServer();

        }

        public override void OnServerError(NetworkConnection conn, int errorCode)
        {
            Debug.Log("OnServerError");

            base.OnServerError(conn, errorCode);

        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("OnClientDisconnect");

            base.OnClientDisconnect(conn);
        }


    

    }
}