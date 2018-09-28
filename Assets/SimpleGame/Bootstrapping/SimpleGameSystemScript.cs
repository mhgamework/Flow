using System;
using System.Collections.Generic;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.Scenes.Persistence;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.BuilderSystem;
using Assets.SimpleGame.Multiplayer;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.VoxelEngine;
using Assets.SimpleGame.WardDrawing;
using UnityEngine;
using UnityEngine.Networking;

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
        private VoxelEngineEditingScript _voxelEngineEditingScript;
        protected internal DynamicObjectsContainerScript dynamicObjectsContainerScript;

        public void Start()
        {
            LevelGameobject.SetActive(false);

            multiplayerSystemScript = createDevMultiplayerSystem();
            multiplayerSystemScript.AutoHostInEditor = true;
            multiplayerSystemScript.PlayerPrefab = playerPrefab.gameObject;
            multiplayerSystemScript.LobbyPrefab = multiplayerLobby;

            multiplayerSystemScript.NetworkManager.OnConnectedToGame += startGame;
            multiplayerSystemScript.NetworkManager.OnDisconnectedFromGame += stopGame;

            setupBuilderSystem();

        }

        void setupBuilderSystem()
        {
            var gameObject = new GameObject("DynamicObjectsContainerScript");
            dynamicObjectsContainerScript = gameObject.AddComponent<DynamicObjectsContainerScript>();
        }

        private void startGame()
        {
            LevelGameobject.SetActive(true);
            var voxelWorldPersister = new VoxelWorldPersister();
            octreeVoxelWorld = voxelWorldPersister.LoadFromFolder(WorldsAssetFolder + "/" + WorldToLoad);



            Instantiate(RenderToTextureSystemPrefab);
            Instantiate(ResourceTypesScriptPrefab);

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
            if (localPlayerCamrea == null) throw new Exception("Should have a player camera!");

            var renderingEngine = VoxelEngineHelpers.CreateVoxelRenderingEngine(
                VoxelRenderingEnginePrefab,
                octreeVoxelWorld,
                Instantiate,
                lodCamera: localPlayerCamrea);
            VoxelRenderingEngine = renderingEngine;


            _voxelEngineEditingScript = GetComponentInChildren<VoxelEngineEditingScript>();
            _voxelEngineEditingScript.Initialize(renderingEngine, octreeVoxelWorld);

            var localPlayerInput = Instantiate(LocalPlayerInputScriptPrefab);
            var wardDrawingModeScript = Instantiate(WardDrawingModeScript);

            localPlayerInput.Initialize(player, renderingEngine, wardDrawingModeScript);


            giveStartItems(player.GetPlayer());


            var levelCallbacks = LevelGameobject.GetComponent<ILevelCallbacks>();
            if (levelCallbacks != null) levelCallbacks.OnLocalPlayerConnected(player);
        }


        private MultiplayerSystemScript createDevMultiplayerSystem()
        {
            return Instantiate(multiplayerSystem, transform);
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

        public T Get<T>() where T : class
        {
            if (typeof(T) == typeof(VoxelEngineEditingScript))
            {
                return _voxelEngineEditingScript as T;
            }

            if (typeof(T) == typeof(DynamicObjectsContainerScript))
                return dynamicObjectsContainerScript as T;

            //            if (typeof(T) == typeof(OctreeVoxelWorld))
            //                return octreeVoxelWorld as T; 
            //
            //            if (typeof(T) == typeof(VoxelRenderingEngineScript))
            //                return VoxelRenderingEngine as T;

            throw new Exception("Cannot find di type: " + typeof(T).Name);



        }
    }
}