using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.Reusable;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen
{
    public class SDFWorldGeneratorScript : MonoBehaviour
    {
        private Dictionary<Color, VoxelMaterial> materials;
        public IVoxelObject[] ChildrenList { get; private set; }
        private List<IVoxelObject> tempList;

        public UniformVoxelData CreateNewChunkData(int size)
        {
            return new UniformVoxelData()
            {
                Data = new Array3D<VoxelData>(new Point3(size,
                    size, size))
            };
        }

        /// <summary>
        /// Cache for the child lookup, to reduce GC alloc
        /// </summary>
        public void UpdateChildrenList()
        {
            Profiler.BeginSample("UpdateChildrenList");
            ChildrenList = GetComponentsInChildren<IVoxelObject>();
            if (tempList == null)
                tempList = new List<IVoxelObject>();
            Profiler.EndSample();
        }

        public void GenerateChunk(Point3 lowerleft, int size, int resolution, UniformVoxelData outData)
        {
            var min = lowerleft;
            var max = lowerleft + new Point3(1, 1, 1) * size * resolution;

            tempList.Clear();
            getObjectsInRange(min, max, tempList);
            outData.isEmpty = tempList.Count == 0;

            if (outData.isEmpty) return;

            Profiler.BeginSample("SampleSDFs");
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            var outDataData = outData.Data;

            if (materials == null)
            {
                materials = new Dictionary<Color, VoxelMaterial>(new ColorEqualityComparer());
            }
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
                            else if (first || density < data.Density )
                            {
                                data.Density = density;
                                Profiler.BeginSample("Material");
                                data.Material = materials.GetOrCreate(color, createMaterial);
                                Profiler.EndSample();
                            }

                            first = false;

                        }

                        outDataData[p] = data;

                    }

            Profiler.EndSample();
        }

        private static VoxelMaterial createMaterial(Color c)
        {
            return new VoxelMaterial() { color = c };
        }

        private void getObjectsInRange(Point3 min, Point3 max, List<IVoxelObject> outList)
        {
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);

            for (var index = 0; index < ChildrenList.Length; index++)
            {
                var v = ChildrenList[index];
                var bounds1 = new Bounds((v.Max + v.Min) * 0.5f, v.Max - v.Min);
                if (!bounds1.Intersects(bounds)) continue;
                outList.Add(v);
            }
        }
    }
}