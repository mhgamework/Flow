using System;
using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample;
using Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample.SystemToMove;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using Assets.Reusable;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.OctreeWorld
{
    /// <summary>
    /// World generation from a Dist Object and bounds
    /// </summary>
    public class DistObject2VoxelWorldGenerator : IWorldGenerator
    {
        private Dictionary<Color, VoxelMaterial> materials = new Dictionary<Color, VoxelMaterial>();

        private ISuggestedSdfInterfaceColor obj;
        private readonly Bounds b;



        public DistObject2VoxelWorldGenerator(ISuggestedSdfInterfaceColor obj, Bounds b)
        {
            this.obj = obj;
            this.b = b;
        }

        public ISuggestedSdfInterfaceColor Obj
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

                    SdfDataColor sdfDataColor;
                    obj.GetSdf(samplePos, out sdfDataColor);

                    var mat = materials.GetOrCreate(sdfDataColor.Color, createMaterial);
                    //worldFunction(p * sampleResolution + start, sampleResolution);
                    outData.Data[p] = new VoxelData(sdfDataColor.Distance, mat);
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