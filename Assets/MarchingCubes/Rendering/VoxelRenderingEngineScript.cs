using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.MarchingCubes.Persistence;
using Assets.MarchingCubes.Rendering.ClipmapsOctree;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.VR;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MarchingCubes.Rendering
{
    public class VoxelRenderingEngineScript : MonoBehaviour
    {
        public bool UseGpuRenderer = false;
        public UnityEngine.ComputeShader GPUShader;
        /// <summary>
        /// Temporary, should be a generic world plugin
        /// </summary>
        public VoxelWorldGenerator World;

        public Camera LODCamera;
        public float LODDistanceFactor = 1.2f;

        //TODO add render scale


        public Text DebugText;

        public Material TemplateMaterial;


        private ClipmapsOctreeService clipmapsOctreeService;


        private bool init = false;
        private AsyncCPUVoxelRenderer renderingService;


        public void Start()
        {


        }
        private void initialize()
        {
            var chunkPoolObject = new GameObject("ChunkPool");
            chunkPoolObject.transform.SetParent(transform);
            var chunkPool = chunkPoolObject.AddComponent<VoxelChunkRendererPoolScript>();

            var rendererObject = new GameObject("Renderer");
            rendererObject.transform.SetParent(transform);

            var world = World.GetWorld(); // createTestWorld();


            IConcurrentVoxelGenerator concurrentGenerator;
            if (UseGpuRenderer)
                concurrentGenerator = createGpuRenderer( world);
            else
                concurrentGenerator = createCPURenderer();
       
            renderingService = new AsyncCPUVoxelRenderer(
                concurrentGenerator,
                chunkPool,
                World.VoxelMaterials,
                world,
                rendererObject.transform,
                TemplateMaterial
            );

            clipmapsOctreeService = new ClipmapsOctreeService(world, renderingService);


        }

        private IConcurrentVoxelGenerator createGpuRenderer(OctreeVoxelWorld world)
        {
            return new GpuVoxelMeshGenerator(GPUShader,world);
        }

        private IConcurrentVoxelGenerator createCPURenderer()
        {

            var marchingCubesService = new MarchingCubesService();
            var meshGenerator = new VoxelChunkMeshGenerator(marchingCubesService);

            var ret =new ConcurrentVoxelGenerator(meshGenerator);
            ret.Start(); // WARN dont forget

            return ret;
        }


        public void Update()
        {
            if (!init)
            {
                initialize();
                init = true;
            }
            clipmapsOctreeService.LODDistanceFactor = LODDistanceFactor;
            clipmapsOctreeService.UpdateRendererState(LODCamera.transform.position / VRSettings.RenderScale);

            if (DebugText)
                DebugText.text = renderingService.UnavailableChunks.ToString();
        }

    }
}