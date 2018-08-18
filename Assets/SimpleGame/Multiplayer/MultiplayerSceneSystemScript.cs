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
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject LobbyPrefab;


        public void Start()
        {
            var mp = Instantiate(multiplayerSystemPrefab);
            mp.PlayerPrefab = playerPrefab;
            mp.LobbyPrefab = LobbyPrefab;
            mp.AutoHostInEditor = AutoHostInEditor;

        }
    }
}