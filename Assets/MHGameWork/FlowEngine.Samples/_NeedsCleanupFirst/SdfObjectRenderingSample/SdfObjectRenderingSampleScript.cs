using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using JetBrains.Annotations;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample
{
    public class SdfObjectRenderingSampleScript : MonoBehaviour
    {
        [SerializeField] private FlowEnginePrefabScript flowEnginePrefab;

        public Color SnowColor = Color.white;
        public Color NoseColor = Color.red;
        public float NoseThick = 0.5f;
        public float NoseLength = 2f;

        public float HeightScale = 1f;

        protected internal OctreeVoxelWorld world;

        protected internal DistObjectVoxelWorldGenerator generator;
        //public FlowEnginePrefabScript flowEngine;

        public float RenderResolution = 0.25f;

        // Use this for initialization
        void Start()
        {
            var translate = new Vector3(10, 10, 10);

            //            generator = new DistObjectVoxelWorldGenerator(new Translation(GetSnowman(), translate),
            //                new Bounds(new Vector3(0, 10, 0) + translate, new Vector3(20, 60, 20)));
            var generator2 = new DistObject2VoxelWorldGenerator(new ExampleNoiseCylinder(), 
                new Bounds(new Vector3(0, 10, 0) + translate, new Vector3(20, 60, 20)));
            world = flowEnginePrefab.CreateWorld(generator2, 16, 5);
            var tempEngine = flowEnginePrefab.CreateFlowEngineLod(world, new LodRendererCreationParams()
            {
                RenderScale = RenderResolution
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                new ClipMapsOctree<OctreeNode>().VisitTopDown(world.Root, n =>
                {
                    //if (!first) return;
                    if (n.VoxelData == null) return; // Non initialized chunk, not loaded yet
                    world.ResetChunk(n, Time.frameCount);
                });
            }
        }

        private void OnValidate()
        {
            if (generator == null) return;
            generator.Obj = new Translation(GetSnowman(), new Vector3(10, 10, 10));
            new ClipMapsOctree<OctreeNode>().VisitTopDown(world.Root, n =>
            {
                //if (!first) return;
                if (n.VoxelData == null) return; // Non initialized chunk, not loaded yet
                world.ResetChunk(n, Time.frameCount);
            });
        }


        public DistObject GetSnowman()
        {
            var noseTrans = new Vector3(0, 15 * HeightScale, NoseLength * 0.5f);
            var noseQuat = Quaternion.AngleAxis(90,Vector3.right);
            var nose =
                new Translation(new Rotation(new Cylinder(NoseThick, NoseLength, NoseColor), noseQuat), noseTrans);
            return new UnionN(
                new Ball(new Vector3(0, 0, 0), 10, SnowColor),
                new Ball(new Vector3(0, 10 * HeightScale, 0), 6, SnowColor),
                new Ball(new Vector3(0, 15 * HeightScale, 0), 4, SnowColor),
                nose);
        }
    }
}