namespace Assets.MarchingCubes.VoxelWorldMVP.Persistence
{
    public interface IWorldSerializer
    {
        void Save(int changesSinceFrame, VoxelWorldAsset asset, OctreeVoxelWorld world);
        VoxelWorldAsset CreateAsset(int minNodeSize);
        VoxelWorldAsset LoadAsset(VoxelWorldAsset asset);
    }
}