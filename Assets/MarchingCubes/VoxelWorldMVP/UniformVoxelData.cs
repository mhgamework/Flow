using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Represents the data of a single voxel
    /// Also contains info on when the data was last changed
    /// </summary>
    public class UniformVoxelData
    {
        public Array3D<VoxelData> Data { get; internal set; }
        public int LastChangeFrame { get; internal set; }   
    }
    public class VoxelData
    {
        public float Density;
        public VoxelMaterial Material;

    }
    public class VoxelMaterial
    {
        public Color color;
    }
}
