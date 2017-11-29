using System.Collections.Generic;

namespace Assets.MarchingCubes.VoxelWorldMVP.Octrees
{
    public interface IConcurrentVoxelGenerator
    {
        void SetRequestedChunks(List<ConcurrentVoxelGenerator.Task> outMissingRenderdataNodes);
        bool HasNodeData(OctreeNode node);
        ConcurrentVoxelGenerator.Result GetNodeData(OctreeNode node);
        void RemoveNodeData(OctreeNode node);
    }
}