using System.Linq;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample;
using Assets.MHGameWork.FlowEngine.SdfWorldGeneration;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using Assets.MHGameWork.FlowGame.ModeBasedInput;
using Assets.MHGameWork.FlowGame.PlayerInput;
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

        // Start is called before the first frame update
        void Start()
        {
//        IWorld w = new SdfBasedWorld() ;

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


            var modeBasedInputSystem = new ModeBasedInputSystem();
            playerInput = new FlowGamePlayerInput(modeBasedInputSystem);
            playerInput.DisableActiveModeKey = KeyCode.Alpha1;
            playerInput.BindInputModeToKey(KeyCode.Alpha2,new ExampleInputMode());
            playerInput.BindInputModeToKey(KeyCode.Alpha3, new ExampleInputMode());
       
        }

        // Update is called once per frame
        void Update()
        {
            playerInput.Update();
        }
    }
}