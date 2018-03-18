using DirectX11;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Interface to implement terrain generation logic
    /// </summary>
    public interface IWorldGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="chunkSize"></param>
        /// <param name="sampleResolution">space between each voxel</param>
        /// <param name="outData"></param>
        /// <returns></returns>
        void Generate(Point3 start, Point3 chunkSize, int sampleResolution, UniformVoxelData outData);
    }
   
}