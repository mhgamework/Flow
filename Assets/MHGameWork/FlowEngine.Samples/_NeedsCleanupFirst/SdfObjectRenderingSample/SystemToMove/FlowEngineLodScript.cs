using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.SimpleGame.VoxelEngine;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample
{
    public class FlowEngineLodScript : MonoBehaviour
    {
        public VoxelRenderingEngineScript VoxelRenderingEngine;

        public OctreeVoxelWorld CreateWorld(IWorldGenerator gen, int chunkSzie, int depth)
        {
            return new OctreeVoxelWorld(gen, chunkSzie, depth);
        }

        public VoxelRenderingEngineScript CreateFlowEngineLod(OctreeVoxelWorld world)
        {
            var engine = VoxelEngineHelpers.CreateVoxelRenderingEngine(VoxelRenderingEngine,world,Instantiate);

            engine.setWorld(world);
            return engine;
        }
    }
}