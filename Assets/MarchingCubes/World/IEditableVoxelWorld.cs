
using System;
using DirectX11;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public interface IEditableVoxelWorld
    {
        void RunKernel1by1(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData, Point3, VoxelData> act, int frame);
        /// <summary>
        /// X * X kernel, but unfolded to 1D by axis
        /// </summary>
        void RunKernelXbyXUnrolled(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData[], Point3, VoxelData> act,int kernelSize, int frame);
    }
}
