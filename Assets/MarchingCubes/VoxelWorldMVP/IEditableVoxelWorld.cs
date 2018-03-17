using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Kernelsize must be an odd number, larger than 1
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxInclusive"></param>
        /// <param name="act"></param>
        /// <param name="kernelSize"></param>
        /// <param name="frame"></param>
        void RunKernel3by3(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData[], Point3, VoxelData> act, int frame);
    }
}
