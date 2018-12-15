using System;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEngine;

namespace Assets.SimpleGame.VoxelEngine
{
    public class VoxelEngineHelpers
    {
        /// <summary>
        /// Since world generation is lazy, force generation of the chunks
        /// </summary>
        public static void EnsureWorldGenerated(OctreeVoxelWorld world)
        {
            new ClipMapsOctree<OctreeNode>().VisitTopDown(world.Root, n => { world.ForceGenerate(n); });
        }

        public static VoxelRenderingEngineScript CreateVoxelRenderingEngine(
            VoxelRenderingEngineScript prefab,
            OctreeVoxelWorld world,
            Func<VoxelRenderingEngineScript, VoxelRenderingEngineScript> unityInstantiate,
            Camera lodCamera = null)
        {
            var renderer = unityInstantiate(prefab);
            renderer.World = new OctreeVoxelWorldScript();
            renderer.World.SetWorldDirectlyFromCodeMode(world);
            renderer.LODCamera = lodCamera ?? Camera.main;
            return renderer;
        }
    }
}