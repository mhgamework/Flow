using DirectX11;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public interface IWorldGenerator
    {
        UniformVoxelData Generate(Point3 start, Point3 chunkSize);
    }
}