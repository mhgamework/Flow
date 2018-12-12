using System;
using Assets.MHGameWork.FlowEngine.Models;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.OctreeWorld
{
    /// <summary>
    /// World generation using a lambda
    /// </summary>
    public class DelegateVoxelWorldGenerator : IWorldGenerator
    {
        private readonly Func<Vector3, int, VoxelData> worldFunction;

        public DelegateVoxelWorldGenerator(Func<Vector3, int, VoxelData> worldFunction)
        {
            this.worldFunction = worldFunction;
        }
        public DelegateVoxelWorldGenerator(Func<Vector3, VoxelData> worldFunction)
        {
            this.worldFunction = (a, b) => worldFunction(a);
        }

        public void Generate(Point3 start, Point3 chunkSize, int sampleResolution, UniformVoxelData outData)
        {
            //Profiler.BeginSample("PRF-GenerateChunk");
            
            outData.Data.ForEach((v, p) =>
            {
                outData.Data[p] = worldFunction(p * sampleResolution + start, sampleResolution);
            });

            //Profiler.EndSample();

         
        }
    }
}