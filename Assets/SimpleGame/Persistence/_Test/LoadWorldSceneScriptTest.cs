using System;
using System.Linq;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.SimpleGame.VoxelEngine;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEngine;

namespace Assets.MarchingCubes.Scenes.Persistence
{
    /// <summary>
    /// Somewhat of a manual helper test, to load and render saved worlds.
    /// Also functions as an example
    /// </summary>
    public class LoadWorldSceneScriptTest : MonoBehaviour
    {
        public string WorldToLoad = "";
        public string WorldsAssetFolder = "Assets/SimpleGame/_SavedWorlds";

        [SerializeField]
        private VoxelRenderingEngineScript VoxelRenderingEnginePrefab;

        public void Start()
        {
            var voxelWorldPersister = new VoxelWorldPersister();
            var loadedWorld = voxelWorldPersister.LoadFromFolder(WorldsAssetFolder + "/" + WorldToLoad);

            VoxelEngineHelpers.CreateVoxelRenderingEngine(VoxelRenderingEnginePrefab, loadedWorld, Instantiate);

        }
    }
}