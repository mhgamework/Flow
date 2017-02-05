using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class UniformVoxelData
    {

        public Array3D<float> Data { get; internal set; }
        public int LastChangeFrame { get; internal set; }   
    }
}
