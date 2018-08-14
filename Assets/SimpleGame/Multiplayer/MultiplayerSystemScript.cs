using UnityEngine;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Group script for the group prefab for the multiplayer system
    /// </summary>
    public class MultiplayerSystemScript:MonoBehaviour
    {
        public CustomNetworkManagerScript NetworkManager
        {
            get { return GetComponentInChildren<CustomNetworkManagerScript>(); }
        }
            
    }
}