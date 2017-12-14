using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MarchingCubes.ComputeShader;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class GpuVoxelMeshGenerator : IVoxelMeshGenerator, IConcurrentVoxelGenerator
    {
        private readonly UnityEngine.ComputeShader shader;
        private readonly OctreeVoxelWorld octreeVoxelWorld;

        private List<Buffers> freeBuffers = new List<Buffers>();

        private Dictionary<OctreeNode, Buffers>[] inProgressBuffers = new Dictionary<OctreeNode, Buffers>[3];
        private int currentInProgressBuffer = 0;

        private Dictionary<OctreeNode, VoxelMeshData> results = new Dictionary<OctreeNode, VoxelMeshData>();


        public GpuVoxelMeshGenerator(UnityEngine.ComputeShader shader, OctreeVoxelWorld octreeVoxelWorld)
        {
            this.shader = shader;
            this.octreeVoxelWorld = octreeVoxelWorld;
            chunkDataSize = octreeVoxelWorld.ChunkSize.X + OctreeVoxelWorld.ChunkOversize;

            for (int i = 0; i < 100; i++)
            {
                freeBuffers.Add(MarchingCubesComputeSceneScript.createBuffers(chunkDataSize));
            }

            for (int i = 0; i < inProgressBuffers.Length; i++)
            {
                inProgressBuffers[i] = new Dictionary<OctreeNode, Buffers>();
            }

        }

        private int chunkDataSize;

        public VoxelMeshData GenerateMeshFromVoxelData(Array3D<VoxelData> data)
        {
            return null;
        }

        private VoxelMeshData retrieveResult(Buffers buffers)
        {
            Profiler.BeginSample("GPUVoxelRetrieve");

            var verts = MarchingCubesComputeSceneScript.retrieveCalculationOutput(buffers);

            Profiler.EndSample();

            int numMeshes = 1;
            List<int[]> indicesList;
            List<Color> color = new List<Color>();
            color.Add(new Color(107 / 255f, 62 / 255f, 37 / 255f, 255 / 255f));


            indicesList = new List<int[]>();
            var indices = new int[verts.Length];
            for (int i = 0; i < verts.Length; i++)
                indices[i] = i;
            indicesList.Add(indices);
            return new VoxelMeshData
            {
                colors = color,
                vertices = new List<Vector3>(verts),
                indicesList = indicesList,
                numMeshes = numMeshes
            };

        }

        private void fireDispatch(Buffers buffers, Point3 coord, int resolution)
        {
            Profiler.BeginSample("GPUVoxelDispatch");
            //var sdf = toSdf(data);
            //sdf = MarchingCubesComputeSceneScript.generateSphereData(chunkDataSize, new Vector3(1, 1, 1) *7, 5, new Vector3());
            //MarchingCubesComputeSceneScript.setSdfToBuffer(sdf, buffers);
            MarchingCubesComputeSceneScript.runCellGatheringKernel(shader, chunkDataSize, buffers, coord, resolution);
            //var cellBufferData = MarchingCubesComputeSceneScript.getCellBufferData(buffers); // Force GPU-CPU sync

            buffers.TrianglesBuffer.SetCounterValue(0);
            MarchingCubesComputeSceneScript.runTriangleKernel(shader, buffers, coord, resolution);
            Profiler.EndSample();
        }

        private float[] toSdf(Array3D<VoxelData> data)
        {
            var sdf = new float[(data.Size.X) * (data.Size.X) * (data.Size.X)];
            int i = 0;
            for (int z = 0; z < (data.Size.X); z++)
                for (int y = 0; y < (data.Size.X); y++)
                    for (int x = 0; x < (data.Size.X); x++)
                    {
                        sdf[i] = -data[new Point3(x, y, z)].Density; // SDF is reversed!
                        //sdf[i] = -1;
                        //if (x == 1 && y == 1 && z == 1)
                        //    sdf[i] = 1;
                        i++;
                    }
            return sdf;
        }

        public void SetRequestedChunks(List<ConcurrentVoxelGenerator.Task> outMissingRenderdataNodes)
        {
            foreach (var done in getDoneRenderingBuffers())
            {
                var data = retrieveResult(done.Value);

                results[done.Key] = data;
                freeBuffers.Add(done.Value);
            }
            getDoneRenderingBuffers().Clear();
            currentInProgressBuffer = (currentInProgressBuffer + 1) % inProgressBuffers.Length;
            var inProgressBuffer = inProgressBuffers[currentInProgressBuffer];
            if (inProgressBuffer.Count > 0) throw new Exception("Algorithm error");
            //foreach(var f in inProgressBuffer)
            //{
            //    freeBuffers.Add(f.Value);
            //}
            //inProgressBuffer.Clear();

            var numChunksPerFrame = 5;
            var numChunks = 0;
            for (int i = 0; i < outMissingRenderdataNodes.Count; i++)
            {
                var task = outMissingRenderdataNodes[i];
                var node = task.dataNode;
                if (results.ContainsKey(node)) continue;
                numChunks++;
                if (numChunks > numChunksPerFrame) break;



                if (freeBuffers.Count == 0) break;
                var buffers = freeBuffers.Last();
                freeBuffers.RemoveAt(freeBuffers.Count - 1);

                fireDispatch(buffers, node.LowerLeft, octreeVoxelWorld.getNodeResolution(node.Depth));
                inProgressBuffer[node] = buffers;
            }

        }

        public bool HasNodeData(OctreeNode node)
        {
            return results.ContainsKey(node);
            return getDoneRenderingBuffers().ContainsKey(node);
        }

        private Dictionary<OctreeNode, Buffers> getInProgress(int num)
        {
            return inProgressBuffers[(num + inProgressBuffers.Length) % inProgressBuffers.Length];
        }

        private Dictionary<OctreeNode, Buffers> getDoneRenderingBuffers()
        {
            return getInProgress(currentInProgressBuffer - 2);
        }

        public ConcurrentVoxelGenerator.Result GetNodeData(OctreeNode node)
        {

            //var data = retrieveResult(getDoneRenderingBuffers()[node]);
            var data = results[node];
            results.Remove(node);
            return new ConcurrentVoxelGenerator.Result
            {
                data = data,
                Frame = node.VoxelData.LastChangeFrame
            };
        }

        public void RemoveNodeData(OctreeNode node)
        {
        }
    }
}
