using System;
using System.Collections.Generic;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using DirectX11;
using NUnit.Framework.Internal;
using UnityEngine;

namespace Assets.MarchingCubes
{
    public class VoxelEngineTests : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Start of tests");
            //TestVoxelWorld_GetVoxelDataArray();
            //TestVoxelWorld_SetVoxelDataArray();
            TestVoxelWorld_SetVoxelDataArray_Padding();
            //TestVoxelWorld_TestKernel3();
            //TestVoxelWorld_TestKernel3_2();
            Debug.Log("End of tests");
        }

        public void TestVoxelWorld_GetVoxelDataArray()
        {
            var world = new OctreeVoxelWorld(
                new DelegateVoxelWorldGenerator(p => new VoxelData(p.x + p.y + p.z, null)), 2, 1);

            var list = new List<OctreeNode>();
            var data = world.getVoxelDataArray(new Point3(0, 0, 0), new Point3(0, 0, 0), list);

            AreEqual(1, data.Length);
            AreEqual(0, data[0].Density);


            data = world.getVoxelDataArray(new Point3(1, 1, 1), new Point3(1, 1, 1), list);

            AreEqual(1, data.Length);
            AreEqual(3, data[0].Density);

            data = world.getVoxelDataArray(new Point3(0, 0, 0), new Point3(1, 1, 1), list);

            AreEqual(8, data.Length);
            AreEqual(0, data[0].Density);
            AreEqual(3, data[7].Density);

            data = world.getVoxelDataArray(new Point3(1, 1, 1), new Point3(2, 2, 2), list);

            AreEqual(8, data.Length);
            AreEqual(3, data[0].Density);
            AreEqual(6, data[7].Density);

            list.Clear();
            data = world.getVoxelDataArray(new Point3(0, 0, 0), new Point3(3, 3, 3), list);

            AreEqual(4 * 4 * 4, data.Length);
            AreEqual(0, data[0].Density);
            AreEqual(3, data[3].Density);
            AreEqual(1, data[4].Density);
            AreEqual(9, data[4 * 4 * 4 - 1].Density);

            AreEqual(9, list.Count);


        }

        public void TestVoxelWorld_SetVoxelDataArray()
        {
            var world = new OctreeVoxelWorld(
                new DelegateVoxelWorldGenerator(p => new VoxelData(p.x + p.y + p.z, null)), 2, 1);

            var allNodes = new List<OctreeNode>();
            allNodes.Add(world.GetNode(new Point3(0, 0, 0), 0));
            allNodes.Add(world.GetNode(new Point3(0, 0, 0), 1));
            allNodes.Add(world.GetNode(new Point3(0, 0, 2), 1));
            allNodes.Add(world.GetNode(new Point3(0, 2, 0), 1));
            allNodes.Add(world.GetNode(new Point3(0, 2, 2), 1));
            allNodes.Add(world.GetNode(new Point3(2, 0, 0), 1));
            allNodes.Add(world.GetNode(new Point3(2, 0, 2), 1));
            allNodes.Add(world.GetNode(new Point3(2, 2, 0), 1));
            allNodes.Add(world.GetNode(new Point3(2, 2, 2), 1));


            var data = new VoxelData[1];
            data[0] = new VoxelData(999, null);

            world.setVoxelDataArray(new Point3(0, 0, 0), new Point3(0, 0, 0), 123, allNodes, data);

            AreEqual(999, world.GetNode(new Point3(0, 0, 0), 0).VoxelData.Data[new Point3(0, 0, 0)].Density);
            AreEqual(123, world.GetNode(new Point3(0, 0, 0), 0).VoxelData.LastChangeFrame);
            AreEqual(999, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(0, 0, 0)].Density);
            AreEqual(123, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.LastChangeFrame);



            data[0] = new VoxelData(555, null);

            world.setVoxelDataArray(new Point3(1, 1, 1), new Point3(1, 1, 1), 456, allNodes, data);

            AreEqual(999, world.GetNode(new Point3(0, 0, 0), 0).VoxelData.Data[new Point3(0, 0, 0)].Density);
            AreEqual(123, world.GetNode(new Point3(0, 0, 0), 0).VoxelData.LastChangeFrame);
            AreEqual(555, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(1, 1, 1)].Density);
            AreEqual(456, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.LastChangeFrame);


            var data2 = new VoxelData[8];
            data2[0] = new VoxelData(1337, null);
            data2[7] = new VoxelData(5012, null);

            world.setVoxelDataArray(new Point3(1, 1, 1), new Point3(2, 2, 2), 789, allNodes, data2);

            AreEqual(5012, world.GetNode(new Point3(0, 0, 0), 0).VoxelData.Data[new Point3(1, 1, 1)].Density);
            AreEqual(789, world.GetNode(new Point3(0, 0, 0), 0).VoxelData.LastChangeFrame);

            AreEqual(1337, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(1, 1, 1)].Density);
            AreEqual(789, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.LastChangeFrame);
            AreEqual(5012, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(2, 2, 2)].Density);

            AreEqual(5012, world.GetNode(new Point3(2, 2, 2), 1).VoxelData.Data[new Point3(0, 0, 0)].Density);
            AreEqual(789, world.GetNode(new Point3(2, 2, 2), 1).VoxelData.LastChangeFrame);


        }

        public void TestVoxelWorld_SetVoxelDataArray_Padding()
        {
            var world = new OctreeVoxelWorld(
                new DelegateVoxelWorldGenerator(p => new VoxelData(p.x + p.y + p.z, null)), 2, 1);

            var allNodes = new List<OctreeNode>();
            allNodes.Add(world.GetNode(new Point3(0, 0, 0), 0));
            allNodes.Add(world.GetNode(new Point3(0, 0, 0), 1));
            allNodes.Add(world.GetNode(new Point3(0, 0, 2), 1));
            allNodes.Add(world.GetNode(new Point3(0, 2, 0), 1));
            allNodes.Add(world.GetNode(new Point3(0, 2, 2), 1));
            allNodes.Add(world.GetNode(new Point3(2, 0, 0), 1));
            allNodes.Add(world.GetNode(new Point3(2, 0, 2), 1));
            allNodes.Add(world.GetNode(new Point3(2, 2, 0), 1));
            allNodes.Add(world.GetNode(new Point3(2, 2, 2), 1));


            var data = new VoxelData[3*3*3];
            data[1+3+9] = new VoxelData(999, null);
            data[0] = new VoxelData(777, null);

            world.setVoxelDataArray(new Point3(0, 0, 0), new Point3(2, 2, 2), 123, allNodes, data,1);

            AreEqual(999, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(1, 1, 1)].Density);

            // Should be not set!
            AreEqual(0, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(0, 0, 0)].Density);
        }

        //public void TestVoxelWorld_TestKernel1()
        //{
        //    var world = new OctreeVoxelWorld(
        //        new DelegateVoxelWorldGenerator(p => new VoxelData(p.x + p.y + p.z, null)), 2, 1);

        //    world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(0, 0, 0)] = new VoxelData(55, null);

        //    world.RunKernelXbyXUnrolled(new Point3(0, 0, 0), new Point3(0, 0, 0), (input, p) =>
        //    {
        //        return new VoxelData(input[0].Density + 1, null);
        //    }, 1, 123);

        //    // One pass for each axis
        //    AreEqual(55 + 1 + 1 + 1, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(0, 0, 0)].Density);

        //}

        public void TestVoxelWorld_TestKernel3()
        {
            var world = new OctreeVoxelWorld(
                new DelegateVoxelWorldGenerator(p =>
                {
                    if (p == new Point3(0, 0, 0))
                        return new VoxelData(27, null);
                    return new VoxelData(0, null);

                }), 2, 1);


            world.RunKernelXbyXUnrolled(new Point3(1, 1, 1), new Point3(1, 1, 1), (input, p) =>
            {
                var d = 0f;
                for (int i = 0; i < 3; i++)
                {
                    d += input[i].Density;
                }
                return new VoxelData(d / 3f, null);
            }, 3, 123);

            // 27/27
            AreEqual(1f, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(1, 1, 1)].Density);

        }

        public void TestVoxelWorld_TestKernel3_2()
        {
            var world = new OctreeVoxelWorld(
                new DelegateVoxelWorldGenerator(p =>
                {
                    if (p == new Point3(0, 0, 0))
                        return new VoxelData(27, null);
                    return new VoxelData(0, null);

                }), 2, 1);


            world.RunKernelXbyXUnrolled(new Point3(1,1, 1), new Point3(3, 3, 3), (input, p) =>
            {
                var d = 0f;
                for (int i = 0; i < 3; i++)
                {
                    d += input[i].Density;
                }
                return new VoxelData(d / 3f, null);
            }, 3, 123);

            // 27/27
            AreEqual(1f, world.GetNode(new Point3(0, 0, 0), 1).VoxelData.Data[new Point3(1, 1, 1)].Density);

        }

        private void AreEqual(int p0, int dataLength)
        {
            if (p0 != dataLength) throw new Exception("Not equal1 " + p0 + " - " + dataLength);
        }
        private void AreEqual(float p0, float dataLength)
        {
            if (p0 != dataLength) throw new Exception("Not equal1 " + p0 + " - " + dataLength);
        }
    }
}