using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MHGameWork.FlowEngine.SdfWorldGeneration
{
    /// <summary>
    /// Generates UniformVoxelData from IVoxelObjects
    /// </summary>
    public class SdfVoxelWorldGenerator : IWorldGenerator
    {
        private VoxelMaterialFactory materialFactory;
        private ISdfObjectWorld sdfObjectWorld;

        private List<IVoxelObject> tempList;

        public SdfVoxelWorldGenerator(ISdfObjectWorld sdfObjectWorld, VoxelMaterialFactory materialFactory)
        {
            this.sdfObjectWorld = sdfObjectWorld;
            this.materialFactory = materialFactory;
            tempList = new List<IVoxelObject>(100);
        }

        public void Generate(Point3 start, Point3 chunkSize, int sampleResolution, UniformVoxelData outData)
        {
            lock (this)
            {
                generate(start, chunkSize.X, sampleResolution, outData, tempList);
            }
        }
   

        public static UniformVoxelData CreateNewChunkData(int size)
        {
            return new UniformVoxelData()
            {
                Data = new Array3D<VoxelData>(new Point3(size,
                    size, size))
            };
        }
        private void generate(Point3 lowerleft, int size, int resolution, UniformVoxelData outData, List<IVoxelObject> tempList)
        {
            var min = lowerleft;
            var max = lowerleft + new Point3(1, 1, 1) * size * resolution;

            tempList.Clear();
            sdfObjectWorld.GetObjectsInRange(min, max, tempList);
            outData.isEmpty = tempList.Count == 0;

            if (outData.isEmpty) return;

            Profiler.BeginSample("SampleSDFs");
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            var outDataData = outData.Data;


            for (int z = 0; z < size; z++)
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        var p = new Point3(x, y, z);
                        var pWorld = new Point3(
                            lowerleft.X + x * resolution,
                            lowerleft.Y + y * resolution,
                            lowerleft.Z + z * resolution);
                        var data = new VoxelData(float.MaxValue, null);
                        var first = true;

                        for (var index = 0; index < tempList.Count; index++)
                        {
                            var o = tempList[index];
                            Color color;
                            float density;

                            o.Sdf(pWorld, data, out density, out color);

                            if (o.Subtract)
                            {
                                if (first)
                                    continue; // Dont do anything if first

                                if (-density > data.Density)
                                    data.Density = -density;
                            }
                            else if (first || density < data.Density)
                            {
                                data.Density = density;
                                Profiler.BeginSample("Material");
                                materialFactory.GetOrCreate(color);

                                Profiler.EndSample();
                            }

                            first = false;

                        }

                        outDataData[p] = data;

                    }

            Profiler.EndSample();
        }

    }
}