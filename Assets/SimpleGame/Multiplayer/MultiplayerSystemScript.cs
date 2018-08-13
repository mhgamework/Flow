using UnityEngine;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Placeholder for the group prefab for the multiplayer system
    /// </summary>
    public class MultiplayerSystemScript:MonoBehaviour
    {
        public MultiplayerNetworkManagerScript NetworkManager
        {
            get { return GetComponentInChildren<MultiplayerNetworkManagerScript>(); }
        }
            
    }
}