using System.Linq;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample;
using Assets.MHGameWork.FlowEngine.SdfWorldGeneration;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.Scenes.BoxLevelGenerator
{
    public class BoxLevelGeneratorSceneScript : MonoBehaviour
    {
        [SerializeField] private FlowEnginePrefabScript flowEnginePrefab;

        [SerializeField] private Transform sdfLevel;

        // Start is called before the first frame update
        void Start()
        {
//        IWorld w = new SdfBasedWorld() ;

            var sdfWorld = new SimpleSdfObjectWorld();

            ;

            sdfWorld.Objects.Add(sdfLevel.Find("All").GetComponent<BaseVoxelObjectScript>());
            sdfWorld.Objects.AddRange(sdfLevel.Find("Extractors").GetComponentsInChildren<BoxVoxelObjectScript>(true).Cast<IVoxelObject>().ToList());
            sdfWorld.Objects.AddRange(sdfLevel.Find("Placeables").GetComponentsInChildren<BoxVoxelObjectScript>(true).Cast<IVoxelObject>().ToList());
            sdfWorld.Objects.AddRange(sdfLevel.Find("Ores").GetComponentsInChildren<BoxVoxelObjectScript>(true).Cast<IVoxelObject>().ToList());

            Debug.Log("Found " + sdfWorld.Objects + " VoxelObjects");

            var world = flowEnginePrefab.CreateWorld(new SdfVoxelWorldGenerator(sdfWorld, new VoxelMaterialFactory()),
                16, 8);

            var engine = flowEnginePrefab.CreateFlowEngineLod(world,
                new LodRendererCreationParams()
                {
                    RenderScale = 1 //0.25f
                });

            sdfLevel.Find("All").gameObject.SetActive(false);
            sdfLevel.Find("Extractors").gameObject.SetActive(false);
            sdfLevel.Find("Placeables").gameObject.SetActive(false);
            sdfLevel.Find("Ores").gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}