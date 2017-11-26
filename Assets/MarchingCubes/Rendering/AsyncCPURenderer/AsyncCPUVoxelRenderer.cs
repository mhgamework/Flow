using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.MarchingCubes.Rendering.AsyncCPURenderer;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.Rendering
{
    /// <summary>
    /// TODO: Hide/Show is not idempotent. Probably now called exactly once correctly by other classes
    /// but is dangerous
    /// TODO: concurrentvoxelgenerator is holding chunks in cache until shown, it could be that chunks
    /// are going into the generator but never being shown, leaking memory
    /// TODO: concurrentvoxelgenerator currently seems to be a single parallel thread, 
    /// which could be improved
    /// </summary>
    public class AsyncCPUVoxelRenderer
    {
        private Material TemplateMaterial;
        private Dictionary<Color, Material> materialsDictionary;

        private ConcurrentVoxelGenerator concurrentVoxelGenerator;
        private readonly VoxelChunkRendererPoolScript chunkPool;
        private readonly OctreeVoxelWorld octreeVoxelWorld;
        private readonly Transform transform;

        private List<ConcurrentVoxelGenerator.Task> tempTaskList = new List<ConcurrentVoxelGenerator.Task>();

        private Dictionary<ChunkCoord, OctreeNode> nodeLookup = new Dictionary<ChunkCoord, OctreeNode>();

        public int UnavailableChunks { get; private set; }


        public AsyncCPUVoxelRenderer(ConcurrentVoxelGenerator concurrentVoxelGenerator,
            VoxelChunkRendererPoolScript chunkPool,
            List<VoxelMaterial> voxelMaterials,
            OctreeVoxelWorld octreeVoxelWorld,
            Transform transform, Material templateMaterial)
        {
            this.concurrentVoxelGenerator = concurrentVoxelGenerator;
            this.chunkPool = chunkPool;
            this.octreeVoxelWorld = octreeVoxelWorld;
            this.transform = transform;
            TemplateMaterial = templateMaterial;

            this.materialsDictionary = voxelMaterials.ToDictionary(v => v.color, c =>
            {
                var mat = new Material(TemplateMaterial);
                mat.color = c.color;
                return mat;
            });

            var t = new Thread(() =>testThread(0));
            t.Start();
            t = new Thread(() => testThread(1));
            t.Start();
            t = new Thread(() => testThread(2));
            t.Start();
        }

        private Queue<ChunkCoord> parallelGen = new Queue<ChunkCoord>();
        void testThread(int num)
        {
            try
            {
                Debug.Log("Start " + num);
                int amount = 10;
                var buf = new List<ChunkCoord>();
                for (; ; )
                {
                    lock (parallelGen)
                    {
                        while (parallelGen.Count == 0)
                            Monitor.Wait(parallelGen);
                        buf.Clear();
                        while (parallelGen.Count != 0 && buf.Count < amount)
                            buf.Add(parallelGen.Dequeue());
                    }
                    for (int i = 0; i < buf.Count; i++)
                    {
                        octreeVoxelWorld.PregenerateChunk(buf[i]);
                        Debug.Log(num + " " + buf[i]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
       

        }

        /// <summary>
        /// Prepare a chunk to be rendered. Should generate all render data offscreen.
        /// Results in CanShowChunk becoming true
        /// </summary>
        /// <param name="tempTaskList"></param>
        public void PrepareShowChunk(List<ChunkCoord> tasks)
        {
            UnavailableChunks = tasks.Count;

            tempTaskList.Clear();
            lock (parallelGen)
            {
                parallelGen.Clear();
            }
            for (int i = 0; i < tasks.Count; i++)
            {
                var c = tasks[i];
                if ( !octreeVoxelWorld.HasChunkDataAvailable(c))
                {
                    lock (parallelGen)
                    {
                        parallelGen.Enqueue(c);
                        Monitor.PulseAll(parallelGen);
                    }
                    continue;
                }
                var node = getNode(c);
                tempTaskList.Add(new ConcurrentVoxelGenerator.Task
                {
                    dataNode = node,
                    Frame = node.VoxelData.LastChangeFrame,
                    chunkData = node.VoxelData.Data
                });
            }
            concurrentVoxelGenerator.SetRequestedChunks(tempTaskList);
        }

        public bool CanShowChunk(ChunkCoord c)
        {
            OctreeNode node;
            if (!tryGetNode(c, out node)) return false; // Node not genned yet
            return concurrentVoxelGenerator.HasNodeData(node);
        }

        /// <summary>
        /// Warning: Allthough this is called show/hide, these methods are currently
        /// probably NOT idempotent.
        /// </summary>
        public VoxelChunkRendererScript ShowChunk(ChunkCoord node, out int frame)
        {
            var result = concurrentVoxelGenerator.GetNodeData(getNode(node));
            concurrentVoxelGenerator.RemoveNodeData(getNode(node));

            var ret = createRenderData(result); ;

            activateRenderdata(node, ret);

            frame = result.Frame;

            return ret;
        }
        /// <summary>
        /// Warn: Not idempotent!!
        /// </summary>
        /// <param name="renderObject"></param>
        public void HideChunk(VoxelChunkRendererScript renderObject)
        {
            chunkPool.ReleaseChunk(renderObject);
        }

        private VoxelChunkRendererScript createRenderData(ConcurrentVoxelGenerator.Result result)
        {
            Profiler.BeginSample("PRF-RequestChunk");

            var comp = chunkPool.RequestChunk();

            Profiler.EndSample();

            Profiler.BeginSample("PRF-SetToUnity");


            comp.AutomaticallyGenerateMesh = false;
            comp.MaterialsDictionary = materialsDictionary;
            comp.setMeshToUnity(result.data);
            comp.transform.SetParent(transform);
            comp.gameObject.SetActive(true);

            Profiler.EndSample();

            return comp;
        }

        private void activateRenderdata(ChunkCoord node, VoxelChunkRendererScript renderDataOrNull)
        {
            var comp = renderDataOrNull;
            comp.MaterialsDictionary = materialsDictionary;
            comp.SetWorldcoords(node.LowerLeft, octreeVoxelWorld.GetNodeSize(node.Depth) / (float)(octreeVoxelWorld.ChunkSize.X)); // TOOD: DANGEROES

            comp.transform.SetParent(transform);
            //comp.gameObject.SetActive(true);
        }


        public int GetLastChangeFrame(ChunkCoord node)
        {
            return getNode(node).VoxelData.LastChangeFrame;
        }

        private bool tryGetNode(ChunkCoord f, out OctreeNode node)
        {
            return nodeLookup.TryGetValue(f, out node);
        }
        private OctreeNode getNode(ChunkCoord f)
        {
            OctreeNode ret;
            if (nodeLookup.TryGetValue(f, out ret)) return ret;
            //if (f.DataNode == null)
            //f.DataNode = VoxelWorld.GetNode(f.LowerLeft, f.Depth);
            //return f.DataNode;
            ret = octreeVoxelWorld.GetNode(f.LowerLeft, f.Depth);
            nodeLookup[f] = ret;
            return ret;
        }

    }
}