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
    }
}
