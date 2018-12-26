using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.SimpleGame.VoxelEngine;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample
{
    public class FlowEnginePrefabScript : MonoBehaviour
    {
        public VoxelRenderingEngineScript VoxelRenderingEngine;

        public OctreeVoxelWorld CreateWorld(IWorldGenerator gen, int chunkSzie, int depth)
        {
            return new OctreeVoxelWorld(gen, chunkSzie, depth);
        }

        public IFlowEngineLodRenderer CreateFlowEngineLod(OctreeVoxelWorld world, LodRendererCreationParams parameters = null) 
        {
            if (parameters == null) parameters = new LodRendererCreationParams();;
            var engine = VoxelEngineHelpers.CreateVoxelRenderingEngine(VoxelRenderingEngine,world,Instantiate);

            engine.setWorld(world);
            engine.RenderScale = parameters.RenderScale;
            return engine;
        }
    }
}