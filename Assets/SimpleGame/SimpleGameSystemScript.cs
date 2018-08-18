using System;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.Scenes.Persistence;
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
        public string WorldToLoad = "";
        public string WorldsAssetFolder = "Assets/SimpleGame/_SavedWorlds";

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


        [Obsolete] public VoxelRenderingEngineScript VoxelRenderingEngine { get; private set; }

        public void Start()
        {
//            var player = LocalPlayerScript.GetInstanceOrNull();
//            if (player == null)
//                throw new Exception("SimpleGameSystem needs a LocalPlayerScript instance in the scene!");
//            

            var voxelWorldPersister = new VoxelWorldPersister();
            var world = voxelWorldPersister.LoadFromFolder(WorldsAssetFolder + "/" + WorldToLoad);

            var hud = Instantiate(HudPrefab);


            Instantiate(RenderToTextureSystemPrefab);
            Instantiate(ResourceTypesScriptPrefab);


            //createLocalPlayer();
            createSky();

            var wardDrawingModeScript = Instantiate(WardDrawingModeScript);


            var mpSystem = Instantiate(multiplayerSystem);
            mpSystem.SetPlayerPrefab(playerPrefab.gameObject);
            mpSystem.AutostartHostIfEditor();

            //Setup local player stuff

            mpSystem.NetworkManager.OnGameStart += () =>
            {
                var player = LocalPlayerScript.Instance;
                player.Initialize(hud);
                var localPlayerCamrea = player.GetCamera();


                var renderingEngine = VoxelEngineHelpers.CreateVoxelRenderingEngine(
                    VoxelRenderingEnginePrefab,
                    world,
                    Instantiate,
                    lodCamera: localPlayerCamrea);
                VoxelRenderingEngine = renderingEngine;

                var localPlayerInput = Instantiate(LocalPlayerInputScriptPrefab);
                localPlayerInput.Initialize(player, renderingEngine, wardDrawingModeScript);


                giveStartItems(player.GetPlayer());
            };
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