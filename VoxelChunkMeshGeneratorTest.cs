using Assets.MarchingCubes;
using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ClassLibrary1
{
    [TestFixture]
    public class VoxelChunkMeshGeneratorTest
    {
        [Test]
        public void TestPerformance()
        {
            var size = 16;
            var data = createData(size);

            var meshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());

            List<Vector3> verts = null;
            int numMeshes = 0;
            List<int[]> indices = null;
            List<Color> colors = null;
            List<Color> vertexColors = null;

            var watch = new Stopwatch();
            watch.Start();

            var times = 500;

            for (int i = 0; i < times; i++)
            {
                
                meshGenerator.generateMeshSeparateMaterials(data, out verts, out numMeshes, out indices,out colors);
            }

            watch.Stop();

            Assert.True(indices.Count > 0);
            Assert.True(verts.Count > 0);

            Console.WriteLine("Time per chunk of size {0}: {1} ms", size, watch.ElapsedMilliseconds / (float)times);
            Console.WriteLine("Time per kernel: {0} µs", watch.ElapsedMilliseconds / (float)times / size / size / size*1000);

        }

        private Array3D<VoxelData> createData(int size)
        {
            var data = new Array3D<VoxelData>(new DirectX11.Point3(size, size, size));

            var mat = new VoxelMaterial() { color = Color.green };

            data.ForEach((v, p) =>
            {
                var vox = new VoxelData()
                {
                    Density = 10000,
                    Material = mat
                };
                data[p] = vox;
            });

            var scale = (float)(size - 1);
            var uniform = new UniformVoxelData() { Data = data };
            var tool = new WorldEditTool();
            // Note that these colors are hardcoded in the algorithm at this point
            tool.addSphere(uniform, new Vector3(0.3f, 0.3f, 0.3f) * scale, 0.2f * scale, new VoxelMaterial() { color = Color.green }); 
            tool.addSphere(uniform, new Vector3(0.7f, 0.3f, 0.7f) * scale, 0.2f * scale, new VoxelMaterial() { color = Color.red });
            tool.addSphere(uniform, new Vector3(0.3f, 0.7f, 0.7f) * scale, 0.2f * scale, new VoxelMaterial() { color = Color.blue });

            return data;
        }
    }
}
