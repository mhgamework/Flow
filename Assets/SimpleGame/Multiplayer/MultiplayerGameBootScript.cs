using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer
{
    public class MultiplayerGameBootScript : MonoBehaviour
    {
        public NetworkManager manager;
        public bool AutoHostInEditor = true;
        public void Start()
        {
            if (Application.isEditor && AutoHostInEditor)
                manager.StartHost();
        }
    }
}