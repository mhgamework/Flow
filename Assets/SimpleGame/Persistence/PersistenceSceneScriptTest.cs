using System;
using System.IO;
using System.Linq;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.MarchingCubes.VoxelWorldMVP.Persistence;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEngine;

namespace Assets.MarchingCubes.Scenes.Persistence
{
    /// <summary>
    /// Test for the voxel persistence functionality in this package.
    /// Functions as a scene test
    /// </summary>
    public class PersistenceSceneScriptTest : MonoBehaviour
    {
        [SerializeField]
        private VoxelRenderingEngineScript VoxelRenderingEnginePrefab;

        public void Start()
        {
            var mat = new VoxelMaterial(Color.red);
            var initialWorld = new OctreeVoxelWorld(new DelegateVoxelWorldGenerator(v => new VoxelData(v.y - 10, mat)), 8, 2);

            ensureWorldGenerated(initialWorld);

            var voxelWorldPersister = new VoxelWorldPersister();
            voxelWorldPersister.SaveToFolder("testData/PersistenceSceneScriptTest/world", initialWorld);


            voxelWorldPersister = new VoxelWorldPersister();
            var loadedWorld = voxelWorldPersister.LoadFromFolder("testData/PersistenceSceneScriptTest/world");

            var voxelRenderingEngine = createVoxelRenderingEngine(VoxelRenderingEnginePrefab, loadedWorld); // loadedWorld

        }

        /// <summary>
        /// Since world generation is lazy, force generation of the chunks
        /// </summary>
        /// <param name="initialWorld"></param>
        private void ensureWorldGenerated(OctreeVoxelWorld world)
        {
            new ClipMapsOctree<OctreeNode>().VisitTopDown(world.Root, n => { world.ForceGenerate(n); });
        }

        private VoxelRenderingEngineScript createVoxelRenderingEngine(VoxelRenderingEngineScript prefab, OctreeVoxelWorld world)
        {
            var renderer = Instantiate(prefab);
            renderer.World = new OctreeVoxelWorldScript();
            renderer.World.SetWorldDirectlyFromCodeMode(world);
            renderer.LODCamera = Camera.main;
            return renderer;
        }

        public void Update()
        {

        }


    }

    public class VoxelWorldPersister
    {
        public VoxelWorldPersister()
        {

        }

        public void SaveToFolder(string folder, OctreeVoxelWorld world)
        {
            Directory.CreateDirectory(folder);

            var serializer = new RuntimeWorldSerializer(folder + "/voxel");

            var asset = serializer.CreateAsset(world.ChunkSize.X);
            serializer.Save(-1, asset, world);
        }

        public OctreeVoxelWorld LoadFromFolder(string folder)
        {
            //var serializer = new RuntimeWorldSerializer(folder + "/voxel");
            var serializer = new RuntimeWorldSerializer("savegame");

            int depth;
            int chunkSize;
            serializer.ReconstructDepthAndChunkSizeFromSave( out chunkSize, out depth);

            var asset = serializer.CreateAsset(chunkSize + OctreeVoxelWorld.ChunkOversize);

            serializer.LoadFromDisk(asset);

            var world = new OctreeVoxelWorld(new PersistenceWorldGenerator(new ConstantVoxelWorldGenerator(0f, null), asset), chunkSize, depth);
            return world;
        }

       
    }
}