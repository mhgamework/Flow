using DirectX11;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Interface to implement terrain generation logic
    /// </summary>
    public interface IWorldGenerator
    {
        UniformVoxelData Generate(Point3 start, Point3 chunkSize);
    }
}