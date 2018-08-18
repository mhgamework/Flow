using System;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.Scenes.Persistence;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.Multiplayer;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.VoxelEngine;
using Assets.SimpleGame.WardDrawing;
using UnityEngine;

namespace Assets.SimpleGame
{
    /// <summary>
    /// The core bootstrapping script for the "SimpleGame" prototype
    /// Probably will be somewhat of a DI container
    /// </summary>
    public class SimpleGameSystemScript : Singleton<SimpleGameSystemScript>
    {
        [Header("World Setup")]
        public string WorldToLoad = "";
        public string WorldsAssetFolder = "Assets/SimpleGame/_SavedWorlds";
        public GameObject LevelGameobject;

        [Header("Game system prefabs")]

        [SerializeField] private VoxelRenderingEngineScript VoxelRenderingEnginePrefab;
        [SerializeField] private GameObject SkyPrefab;
        [SerializeField] private SimpleGameHUDScript HudPrefab;
        [SerializeField] private SimpleGameInputScript LocalPlayerInputScriptPrefab;
        [SerializeField] private WardDrawingModeScript WardDrawingModeScript;

        [SerializeField] private RenderToTextureScript RenderToTextureSystemPrefab;

        [SerializeField] private ResourceTypesScript ResourceTypesScriptPrefab;
        //SimpleGameHUDScript

        [SerializeField] private MultiplayerSystemScript multiplayerSystem;
        [SerializeField] private PlayerScript playerPrefab;
        [SerializeField] private GameObject multiplayerLobby;


        [Obsolete] public VoxelRenderingEngineScript VoxelRenderingEngine { get; private set; }


        private MultiplayerSystemScript multiplayerSystemScript;
        private OctreeVoxelWorld octreeVoxelWorld;


        public void Start()
        {
            LevelGameobject.SetActive(false);

            multiplayerSystemScript = createDevMultiplayerSystem();
            multiplayerSystemScript.AutoHostInEditor = false;
            multiplayerSystemScript.PlayerPrefab = playerPrefab.gameObject;
            multiplayerSystemScript.LobbyPrefab = multiplayerLobby;

            multiplayerSystemScript.NetworkManager.OnConnectedToGame += startGame;
            multiplayerSystemScript.NetworkManager.OnDisconnectedFromGame += stopGame;


        }
        private void startGame()
        {
            LevelGameobject.SetActive(true);
            var voxelWorldPersister = new VoxelWorldPersister();
            octreeVoxelWorld = voxelWorldPersister.LoadFromFolder(WorldsAssetFolder + "/" + WorldToLoad);



            Instantiate(RenderToTextureSystemPrefab);
            Instantiate(ResourceTypesScriptPrefab);


            //createLocalPlayer();
            createSky();



         

        }
        private void stopGame()
        {
            throw new NotImplementedException();
        }

        public void OnLocalPlayerCreated(LocalPlayerScript player)
        {
            var hud = Instantiate(HudPrefab);

            player.Initialize(hud);
            var localPlayerCamrea = player.GetCamera();


            var renderingEngine = VoxelEngineHelpers.CreateVoxelRenderingEngine(
                VoxelRenderingEnginePrefab,
                octreeVoxelWorld,
                Instantiate,
                lodCamera: localPlayerCamrea);
            VoxelRenderingEngine = renderingEngine;

            var localPlayerInput = Instantiate(LocalPlayerInputScriptPrefab);
            var wardDrawingModeScript = Instantiate(WardDrawingModeScript);

            localPlayerInput.Initialize(player, renderingEngine, wardDrawingModeScript);


            giveStartItems(player.GetPlayer());


        }


        private MultiplayerSystemScript createDevMultiplayerSystem()
        {
            return Instantiate(multiplayerSystem,transform);
        }

        private void giveStartItems(PlayerScript player)
        {
            player.StoreItems("digtool", 1);
        }

        private void createSky()
        {
            Instantiate(SkyPrefab);
        }


        private void createLocalPlayer()
        {
            //new LocalPlayerScript();

            //createLocalInput();
        }

        private void createLocalInput()
        {
        }

        private void createPlayerCamera()
        {
        }

        private void createPlayerEntity(bool isLocal = true)
        {
        }
    }
}