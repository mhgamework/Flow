using System;
using System.Linq;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.SimpleGame.VoxelEngine;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEngine;

namespace Assets.MarchingCubes.Scenes.Persistence
{
    /// <summary>
    /// Tests serializaing and deserializing a plane voxel world
    /// Test will generate a plane world, save it and load it.
    /// 
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

            VoxelEngineHelpers.EnsureWorldGenerated(initialWorld);

            var voxelWorldPersister = new VoxelWorldPersister();
            voxelWorldPersister.SaveToFolder("tempTestData/PersistenceSceneScriptTest/world", initialWorld);


            voxelWorldPersister = new VoxelWorldPersister();
            var loadedWorld = voxelWorldPersister.LoadFromFolder("tempTestData/PersistenceSceneScriptTest/world");

            var voxelRenderingEngine = VoxelEngineHelpers.CreateVoxelRenderingEngine(VoxelRenderingEnginePrefab, loadedWorld, Instantiate);

        }
    }
}