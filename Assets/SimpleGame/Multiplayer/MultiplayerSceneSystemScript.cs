using UnityEngine;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// The bootstrap script of the MultiplayerScene.
    /// </summary>
    public class MultiplayerSceneSystemScript : MonoBehaviour
    {
        [SerializeField] private bool AutoHostInEditor = true;
        [SerializeField] private MultiplayerSystemScript multiplayerSystemPrefab;


        public void Start()
        {
            var mp = Instantiate(multiplayerSystemPrefab);

            if (Application.isEditor && AutoHostInEditor)
                mp.NetworkManager.StartHost();
        }
    }
}