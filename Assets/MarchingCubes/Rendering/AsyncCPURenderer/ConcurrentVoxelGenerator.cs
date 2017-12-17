using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Assets.Reusable.Threading;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.VoxelWorldMVP.Octrees
{
    /// <summary>
    /// TODO: The thread inside is never stopped!!
    /// Could make all the operations non-blocking by adding queues in between and making it lock free.
    ///   Core idea is that one of the threads should not be affected by synchronization overhead. Not sure which one atm
    ///   Can we reduce synchronization overhead?
    /// </summary>
    public class ConcurrentVoxelGenerator : IConcurrentVoxelGenerator
    {
        private int stopped;
        private Thread t;

        private Dictionary<OctreeNode, Result> cache = new Dictionary<OctreeNode, Result>();

        private List<Task> taskList = new List<Task>();


        private Stack<VoxelMeshData> meshDataPool = new Stack<VoxelMeshData>();

        public ConcurrentVoxelGenerator(VoxelChunkMeshGenerator meshGenerator, bool debugDisableThreading)
        {
            this.meshGenerator = meshGenerator;
            this.debugDisableThreading = debugDisableThreading;
        }

        // MUST BE CALLED!!
        public void Start()
        {
            if (debugDisableThreading) return;
            t = new Thread(anderProcessorProgrammake);
            stopped = 0;
            t.Start();
        }

        public void SetRequestedChunks(List<Task> outMissingRenderdataNodes)
        {
            lock (this)
            {
                taskList.Clear();
                taskList.AddRange(outMissingRenderdataNodes);
                Monitor.Pulse(this);
            }
        }


        public void anderProcessorProgrammake()
        {
            var w = new Stopwatch();

            int i = 0;

            while (stopped == 0)
            {
                generateMeshThread(w);
            }
        }

        private void generateMeshThread(Stopwatch w)
        {
            Task task = new Task();
            VoxelMeshData data = null;

            bool valid = false;
            while (!valid)
            {
                lock (this)
                {
                    if (debugDisableThreading && taskList.Count == 0) return;
                    while (taskList.Count == 0) Monitor.Wait(this);
                    task = taskList[taskList.Count - 1];
                    taskList.RemoveAt(taskList.Count - 1);

                    if (!cache.ContainsKey(task.dataNode)) valid = true;
                    data = takeFromPool();
                }
            }
            //for (int j = 0; j < 20 && i < tasks.Length; j++)
            {
                //i++;
                data.Clear();
                var result = generateMeshTask(task, w, data);
                //Debug.Log(w.ElapsedMilliseconds);
                lock (this)
                {
                    cache[task.dataNode] = result;
                }
            }
        }

        private VoxelMeshData takeFromPool()
        {
            lock (this)
            {
                if (meshDataPool.Count == 0)
                {
                    MainThreadDispatcher.Instance.Dispatch(() =>
                    {
                        Profiler.BeginSample("Allocate Mesh Data");
                        for (int i = 0; i < 1000; i++)
                        {
                            meshDataPool.Push(VoxelMeshData.CreatePreallocated());
                        }
                        Profiler.EndSample();
                    });
                }
                return meshDataPool.Pop();
            }
        }

        private Stopwatch debugStopwatch = new Stopwatch();

        public void Update()
        {
            Profiler.BeginSample("ConcurrentVoxelGenerator-GenerateMeshThread");

            generateMeshThread(debugStopwatch);

            Profiler.EndSample();
        }

        /// <summary>
        /// Release the mesh data back into the pool.
        /// DONT KEEP THE HANDLE AROUND!
        /// </summary>
        /// <param name="resultData"></param>
        public void ReleaseChunkData(VoxelMeshData resultData)
        {
            lock (this)
            {
                meshDataPool.Push(resultData);
            }
        }


        public bool HasNodeData(OctreeNode node)
        {
            lock (this)
                return cache.ContainsKey(node);
        }

        public Result GetNodeData(OctreeNode node)
        {
            lock (this)
                return cache[node];
        }

        public void RemoveNodeData(OctreeNode node)
        {
            lock (this)
            {
                cache.Remove(node);
            }
        }

        private VoxelChunkMeshGenerator meshGenerator;
        private readonly bool debugDisableThreading;

        private VoxelMeshData firstData;

     

        private Result generateMeshTask(Task task, Stopwatch w,VoxelMeshData voxelMeshData)
        {
            w.Reset();
            w.Start();
            //if (firstData == null)
             meshGenerator.GenerateMeshFromVoxelData(task.chunkData, voxelMeshData); // DANGEROES multithreaded
            w.Stop();
            return new Result
            {
                data = voxelMeshData,
                Frame = task.Frame,
                //node = task.node
            };
        }


        public struct Task
        {
            //public RenderOctreeNode node;
            public OctreeNode dataNode;
            public int Frame;
            public Array3D<VoxelData> chunkData;
        }

        public struct Result
        {
            public VoxelMeshData data;
            public int Frame;
            //public RenderOctreeNode node;
        }
    }
}
