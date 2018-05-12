using System;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.Scenes.Persistence;
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

        [Obsolete]
        public VoxelRenderingEngineScript VoxelRenderingEngine { get; private set; }

        public void Start()
        {

            var player = LocalPlayerScript.GetInstanceOrNull();
            if (player == null)
                throw new Exception("SimpleGameSystem needs a LocalPlayerScript instance in the scene!");
            

            var voxelWorldPersister = new VoxelWorldPersister();
            var world = voxelWorldPersister.LoadFromFolder(WorldsAssetFolder + "/" + WorldToLoad);

            var hud = Instantiate(HudPrefab);

            player.Initialize(hud);
            var localPlayerCamrea = player.GetCamera();


            var renderingEngine = VoxelEngineHelpers.CreateVoxelRenderingEngine(
                VoxelRenderingEnginePrefab,
                world,
                Instantiate,
                lodCamera: localPlayerCamrea);
            VoxelRenderingEngine = renderingEngine;


            //createLocalPlayer();
            createSky();

            var wardDrawingModeScript = Instantiate(WardDrawingModeScript);

            var localPlayerInput = Instantiate(LocalPlayerInputScriptPrefab);
            localPlayerInput.Initialize(player,renderingEngine,wardDrawingModeScript);

            Instantiate(RenderToTextureSystemPrefab);
            Instantiate(ResourceTypesScriptPrefab);

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