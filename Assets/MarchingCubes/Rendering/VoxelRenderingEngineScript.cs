using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.Persistence;
using Assets.MarchingCubes.Rendering.ClipmapsOctree;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.VR;
using UnityEngine;

namespace Assets.MarchingCubes.Rendering
{
    public class VoxelRenderingEngineScript : MonoBehaviour
    {
        /// <summary>
        /// Temporary, should be a generic world plugin
        /// </summary>
        public TestVoxelWorldScript World;

        public Camera LODCamera;
        public float LODDistanceFactor = 1.2f;

        //TODO add render scale


        
        public Material TemplateMaterial;


        private ClipmapsOctreeService clipmapsOctreeService;
        

        private bool init = false;


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


            var marchingCubesService = new MarchingCubesService();
            var meshGenerator = new VoxelChunkMeshGenerator(marchingCubesService);

            var concurrentGenerator = new ConcurrentVoxelGenerator(meshGenerator);
            var renderingService = new AsyncCPUVoxelRenderer(
                concurrentGenerator,
                chunkPool,
                World.VoxelMaterials,
                world,
                rendererObject.transform,
                TemplateMaterial
            );

            clipmapsOctreeService = new ClipmapsOctreeService(world, renderingService);


            concurrentGenerator.Start(); // WARN dont forget
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

        }
      
    }
}