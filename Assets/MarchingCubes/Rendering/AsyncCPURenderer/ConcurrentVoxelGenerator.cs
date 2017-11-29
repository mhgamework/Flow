using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

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


        public ConcurrentVoxelGenerator(VoxelChunkMeshGenerator meshGenerator)
        {
            this.meshGenerator = meshGenerator;
        }

        // MUST BE CALLED!!
        public void Start()
        {
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
            Task task = new Task();

            while (stopped == 0)
            {
                bool valid = false;
                while (!valid)
                {
                    lock (this)
                    {
                        while (taskList.Count == 0) Monitor.Wait(this);
                        task = taskList[taskList.Count - 1];
                        taskList.RemoveAt(taskList.Count - 1);

                        if (!cache.ContainsKey(task.dataNode)) valid = true;
                    }

                }
                //for (int j = 0; j < 20 && i < tasks.Length; j++)
                {
                    //i++;

                    var result = generateMeshTask(task, w);
                    //Debug.Log(w.ElapsedMilliseconds);
                    lock (this)
                    {
                        cache[task.dataNode] = result;
                    }
                }

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

        private VoxelMeshData firstData;

     

        private Result generateMeshTask(Task task, Stopwatch w)
        {
            w.Reset();
            w.Start();
            //if (firstData == null)
            firstData = VoxelChunkRendererScript.generateMesh(meshGenerator, task.chunkData); // DANGEROES multithreaded
            w.Stop();
            return new Result
            {
                data = firstData,
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
