using System;
using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using Assets.Reusable;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.OctreeWorld
{
    /// <summary>
    /// World generation from a Dist Object and bounds
    /// </summary>
    public class DistObjectVoxelWorldGenerator : IWorldGenerator
    {
        private Dictionary<Color, VoxelMaterial> materials = new Dictionary<Color, VoxelMaterial>();

        private DistObject obj;
        private readonly Bounds b;



        public DistObjectVoxelWorldGenerator(DistObject obj, Bounds b)
        {
            this.obj = obj;
            this.b = b;
        }

        public DistObject Obj
        {
            get { return obj; }
            set { obj = value; }
        }

        public void Generate(Point3 start, Point3 chunkSize, int sampleResolution, UniformVoxelData outData)
        {
            lock (this)
            {
                //Profiler.BeginSample("PRF-GenerateChunk");

                outData.Data.ForEach((v, p) =>
                {
                    var samplePos = p * sampleResolution + start;

                    if (!b.Contains(samplePos))
                    {
                        outData.Data[p] = new VoxelData(float.MaxValue, new VoxelMaterial(Color.black));
                        return;
                    }
                    var d = obj.Sdf(samplePos);
                    var c = obj.Color(samplePos);

                    var mat = materials.GetOrCreate(c, createMaterial);
                    //worldFunction(p * sampleResolution + start, sampleResolution);
                    outData.Data[p] = new VoxelData(d, mat);
                });

                //Profiler.EndSample();
            }
        }

        private static VoxelMaterial createMaterial(Color c)
        {
            return new VoxelMaterial() {color = c};
        }
    }
}