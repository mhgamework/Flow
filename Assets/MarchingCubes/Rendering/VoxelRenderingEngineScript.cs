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
        public Camera LODCamera;
        public float LODDistanceFactor = 1.2f;

        //TODO add render scale


        public List<Color> MaterialColors;
        public Material TemplateMaterial;


        private ClipmapsOctreeService clipmapsOctreeService;
        private List<VoxelMaterial> voxelMaterials;




        public void Start()
        {
            var chunkPoolObject = new GameObject("ChunkPool");
            chunkPoolObject.transform.SetParent(transform);
            var chunkPool = chunkPoolObject.AddComponent<VoxelChunkRendererPoolScript>();

            var rendererObject = new GameObject("Renderer");
            rendererObject.transform.SetParent(transform);

            var world = createTestWorld();

            voxelMaterials = MaterialColors.Select(c => new VoxelMaterial { color = c }).ToList();

            var marchingCubesService = new MarchingCubesService();
            var meshGenerator = new VoxelChunkMeshGenerator(marchingCubesService);

            var concurrentGenerator = new ConcurrentVoxelGenerator(meshGenerator);
            var renderingService = new VoxelChunkRendererService(
                concurrentGenerator,
                chunkPool,
                voxelMaterials,
                world,
                rendererObject.transform,
                TemplateMaterial
                );

            clipmapsOctreeService = new ClipmapsOctreeService(world, renderingService);


            concurrentGenerator.Start(); // WARN dont forget

        }

        private OctreeVoxelWorld createTestWorld()
        {
            var obj = JasperTest.createSkull();
            //obj = new Box(4, 4, 4);
            IWorldGenerator persistence = new DelegateVoxelWorldGenerator(p => SDFJasper((p) / 100f - Vector3.one * 0.4f, obj));
            int chunkSize = 8;
            int worldDepth = 5;
            var world = new OctreeVoxelWorld(persistence, chunkSize, worldDepth);
            return world;
        }

        public void Update()
        {
            clipmapsOctreeService.LODDistanceFactor = LODDistanceFactor;
            clipmapsOctreeService.UpdateRendererState(LODCamera.transform.position / VRSettings.RenderScale);

        }

        private VoxelData FlatWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = arg1.y - 10,
                Material = voxelMaterials[0]
            };
        }

        private VoxelData LowerleftSphereWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = SdfFunctions.Sphere(arg1, new Vector3(0, 0, 0), 8 * 3),
                Material = voxelMaterials[0]
            };
        }

        private VoxelData SDFJasper(Vector3 arg1, DistObject distObj)
        {
            var density = distObj.Sdf(arg1);
            return new VoxelData()
            {
                Density = density,
                Material = voxelMaterials[0]
            };
        }

    }
}