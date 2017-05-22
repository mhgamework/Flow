using System;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// World generation using a lambda
    /// </summary>
    public class ConstantVoxelWorldGenerator : IWorldGenerator
    {
        private float density;
        VoxelMaterial material;

        public ConstantVoxelWorldGenerator(float density, VoxelMaterial material)
        {
            this.density = density;
            this.material = material;
        }

        private UniformVoxelData first = null;

        public UniformVoxelData Generate(Point3 start, Point3 chunkSize, int sampleResolution)
        {
            //if (first!= null) return first;
            var ret = new UniformVoxelData();
            ret.Data = new Array3D<VoxelData>(chunkSize);
            ret.Data.ForEach((v, p) =>
            {
                ret.Data[p] = new VoxelData()
                {
                    Material = material,
                    Density = density
                };
            });
            //first = ret;
            return ret;
        }
    }
}