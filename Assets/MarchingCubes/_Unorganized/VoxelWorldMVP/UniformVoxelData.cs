using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Represents the data of a single voxel
    /// Also contains info on when the data was last changed
    /// </summary>
    public class UniformVoxelData
    {
        public Array3D<VoxelData> Data { get;  set; }
        public int LastChangeFrame { get;  set; }

        public bool isEmpty;
    }
}
