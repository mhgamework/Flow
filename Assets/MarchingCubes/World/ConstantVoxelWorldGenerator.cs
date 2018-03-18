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

        public void Generate(Point3 start, Point3 chunkSize, int sampleResolution, UniformVoxelData outDat)
        {
            //if (first!= null) return first;
            outDat.Data.ForEach((v, p) =>
            {
                outDat.Data[p] = new VoxelData()
                {
                    Material = material,
                    Density = density
                };
            });
            //first = ret;
        }
    }
}