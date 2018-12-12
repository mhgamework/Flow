using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup.Domain;

namespace Assets.MHGameWork.FlowEngine.Rendering.AsyncCPURenderer
{
    public interface IConcurrentVoxelGenerator
    {
        void SetRequestedChunks(List<ConcurrentVoxelGenerator.Task> outMissingRenderdataNodes);
        bool HasNodeData(OctreeNode node);
        ConcurrentVoxelGenerator.Result GetNodeData(OctreeNode node);
        void RemoveNodeData(OctreeNode node);
        void Update();
        void ReleaseChunkData(VoxelMeshData resultData);
    }
}