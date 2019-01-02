using System;
using System.Linq;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample;
using Assets.MHGameWork.FlowEngine.SdfWorldGeneration;
using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using Assets.MHGameWork.FlowGame.DI;
using Assets.MHGameWork.FlowGame.Domain;
using Assets.MHGameWork.FlowGame.ModeBasedInput;
using Assets.MHGameWork.FlowGame.PlayerInput;
using Assets.MHGameWork.FlowGame.PlayerInputting.Interacting;
using Assets.MHGameWork.FlowGame.PlayerStating;
using Assets.MHGameWork.FlowGame.UI;
using Assets.MHGameWork.FlowGame.UnityEditorVoxelUtils;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.Scenes.BoxLevelGenerator
{
    /// <summary>
    /// Bootstrap script for the BoxLevelGeneratorScene
    /// </summary>
    public class BoxLevelGeneratorSceneScript : MonoBehaviour
    {
        [SerializeField] private FlowEnginePrefabScript flowEnginePrefab;

        [SerializeField] private FlowGameSdfVoxelLevelScript sdfLevel;

        private FlowGamePlayerInput playerInput;
        protected internal FlowGameInteractionSystem flowGameInteractionSystem;

        // Start is called before the first frame update
        void Start()
        {
            var uiScript = FindObjectOfType<FlowGameUiScript>();
            if (!uiScript) throw new Exception("Should have a FlowGameUiScript in scene");

            var sdfWorld = sdfLevel.CreateSdfObjectWorld();
            sdfLevel.HideEditorMeshes();

            var world = flowEnginePrefab.CreateWorld(new SdfVoxelWorldGenerator(sdfWorld, new VoxelMaterialFactory()),
                16, 8);

            var engine = flowEnginePrefab.CreateFlowEngineLod(world,
                new LodRendererCreationParams()
                {
                    RenderScale = 1 //0.25f
                });

            engine.LodDistanceFactor = 0.8f;
            FlowGameServiceProvider.Instance.RegisterService(engine);
            FlowGameServiceProvider.Instance.RegisterService((VoxelRenderingEngineScript)engine); // Hacky temp, needs to be removed when proper physics support i guess

            var globalResources = new PlayerGlobalResourcesRepository();


            flowGameInteractionSystem = new FlowGameInteractionSystem();
            FlowGameServiceProvider.Instance.RegisterService(flowGameInteractionSystem);

            var modeBasedInputSystem = new ModeBasedInputSystem();
            playerInput = new FlowGamePlayerInput(modeBasedInputSystem, flowGameInteractionSystem);
            playerInput.DisableActiveModeKey = KeyCode.Alpha1;
            playerInput.BindInputModeToKey(KeyCode.Alpha2,new ExampleInputMode(),"does nothing");
            playerInput.BindInputModeToKey(KeyCode.Alpha3, new ExampleInputMode(),"does even less");
       
            uiScript.SetProvider(new WiredFlowGameUiProvider(playerInput, globalResources));

            FlowGameServiceProvider.Instance.RegisterService(globalResources);

            globalResources.SetMaxResourceAmount(ResourceTypeFactory.MagicCrystals, 100);
            globalResources.SetMaxResourceAmount(ResourceTypeFactory.Rock, 50);
            globalResources.SetMaxResourceAmount(ResourceTypeFactory.Firestone, 10);

                

        }

        // Update is called once per frame
        void Update()
        {
            playerInput.Update();
            flowGameInteractionSystem.Update();
        }
    }
}