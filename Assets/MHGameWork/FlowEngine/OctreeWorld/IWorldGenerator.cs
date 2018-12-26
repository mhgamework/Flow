using Assets.MHGameWork.FlowEngine.Models;
using DirectX11;

namespace Assets.MHGameWork.FlowEngine.OctreeWorld
{
    /// <summary>
    /// Interface to implement terrain generation logic
    /// TODO: this maybe better named: IWorldSampler
    /// TODO: probably also useful to include a change notification, so it can be used for push/pull world generation
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