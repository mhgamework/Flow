using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.MarchingCubes.Persistence;
using Assets.MarchingCubes.Rendering.ClipmapsOctree;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.VR;
using DirectX11;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MarchingCubes.Rendering
{
    public class VoxelRenderingEngineScript : MonoBehaviour
    {
        public float RenderScale = 1;
        public bool UseGpuRenderer = false;
        public UnityEngine.ComputeShader GPUShader;
        /// <summary>
        /// Temporary, should be a generic world plugin
        /// </summary>
        public VoxelWorldGenerator World;

        public Camera LODCamera;
        public float LODDistanceFactor = 1.2f;

        //TODO add render scale

        public AnimationCurve LodDistanceCurve;
        public float LodDistanceCurveEnd = 1;

        public Text DebugText;

        public Material TemplateMaterial;
        public Material VertexColorMaterial;

        public bool enableMultithreading = true;

        private ClipmapsOctreeService clipmapsOctreeService;


        private bool init = false;
        private AsyncCPUVoxelRenderer renderingService;

        IConcurrentVoxelGenerator concurrentGenerator;
        private OctreeVoxelWorld octreeVoxelWorld;


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

            octreeVoxelWorld = World.CreateNewWorld();


            if (UseGpuRenderer)
                concurrentGenerator = createGpuRenderer( octreeVoxelWorld);
            else
                concurrentGenerator = createCPURenderer();
       
            renderingService = new AsyncCPUVoxelRenderer(
                concurrentGenerator,
                chunkPool,
                World.VoxelMaterials,
                octreeVoxelWorld,
                rendererObject.transform,
                this,
                !enableMultithreading
            );

            clipmapsOctreeService = new ClipmapsOctreeService(octreeVoxelWorld, renderingService,LodDistanceCurve, LodDistanceCurveEnd);


        }

        public OctreeVoxelWorld GetWorld()
        {
            return octreeVoxelWorld;
        }


        private IConcurrentVoxelGenerator createGpuRenderer(OctreeVoxelWorld world)
        {
            return new GpuVoxelMeshGenerator(GPUShader,world);
        }

        private IConcurrentVoxelGenerator createCPURenderer()
        {

            var marchingCubesService = new MarchingCubesService();
            var meshGenerator = new VoxelChunkMeshGenerator(marchingCubesService);

            var ret =new ConcurrentVoxelGenerator(meshGenerator,!enableMultithreading);
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
            clipmapsOctreeService.UpdateRendererState(LODCamera.transform.position / RenderScale);

            concurrentGenerator.Update();

            renderingService.Update();

            if (DebugText)
                DebugText.text = renderingService.UnavailableChunks.ToString();
        }

        public Vector3 ToVoxelSpace(Vector3 min)
        {
            return min / RenderScale;
        }
    }
}