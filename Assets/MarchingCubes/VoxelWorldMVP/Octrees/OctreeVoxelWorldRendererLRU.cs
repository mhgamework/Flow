using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Asyunc LRU cache version, was not working as i wanted, so is here for backup/reference purposes
    /// Can be removed when a better version of this exists
    /// Goal was to reduce edit to rendering lag, parallelize chunk creation and remove holes/flashes when updating
    /// 
    /// Holds state for renderers of the voxel world
    /// Responsible for updating the renderers of the voxel world
    /// 
    /// This IS a unity component, to create some confusion!
    /// Probably makes more sense
    /// </summary>
    public class OctreeVoxelWorldRendererLRU : MonoBehaviour
    {
        public Material TemplateMaterial;
        private Dictionary<Color, Material> materialsDictionary;

        public bool ShowOctree = false;

        public Transform Container;

        // Set by some script
        public OctreeVoxelWorld VoxelWorld;

        private RenderOctreeNode root;
        private ClipMapsOctree<RenderOctreeNode> helper = new ClipMapsOctree<RenderOctreeNode>();
        private LineManager3D manager = new LineManager3D();

        public float LODDistanceFactor = 1.2f;

        private AsyncLRUCache<NodeAndVersion, VoxelChunkRenderer> renderDataCache;
        public int RenderCacheSize = 100;

        private VoxelChunkMeshGenerator meshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());

        public Transform ChunkCache;
        public Transform VisibleChunks;

        //private VoxelChunkMeshGenerator voxelChunkMeshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());

        private Thread t;
        private int stopped;
        public void Start()
        {
            renderDataCache = new AsyncLRUCache<NodeAndVersion, VoxelChunkRenderer>(RenderCacheSize, removeCachedData);
            t = new Thread(anderProcessorProgrammake);
            stopped = 0;
            t.Start();

        }

        private void removeCachedData(NodeAndVersion node, VoxelChunkRenderer data)
        {
            Destroy(data.gameObject);
        }

        public void OnDestroy()
        {
            Interlocked.Increment(ref stopped);
        }

        public void Init(OctreeVoxelWorld world, List<VoxelMaterial> voxelMaterials)
        {
            VoxelWorld = world;
            root = helper.Create(VoxelWorld.Root.Size, VoxelWorld.Root.LowerLeft);

            this.materialsDictionary = voxelMaterials.ToDictionary(v => v.color, c =>
              {
                  var mat = new Material(TemplateMaterial);
                  mat.color = c.color;
                  return mat;
              });

        }



        /// <summary>
        /// Walk the tree, request chunks that should be visible from the cache, and built a tree using what is available in the cache
        /// </summary>
        public bool UpdateQuadtreeClipmaps(RenderOctreeNode node, Vector3 cameraPosition, int minNodeSize, ref int numRequestedFromCache, float distanceFactor = 1.2f)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + Vector3.one * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            // This node might be needed request it in cache
            var renderDataOrNull = renderDataCache.Get(new NodeAndVersion(node, getNode(node).VoxelData.LastChangeFrame));
            numRequestedFromCache++;


            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            if (dist > node.Size * distanceFactor)
            {
                // This is a valid node size at this distance, so remove all children
                helper.Merge(node);

                return trySetRenderData(node, renderDataOrNull);
            }
            else
            {
                if (node.Children == null)
                    helper.Split(node, false, minNodeSize);

                if (node.Children == null)
                {
                    // Minlevel
                    return trySetRenderData(node, renderDataOrNull);
                }
            }

            var areChildrenHoleless = true;
            for (int i = 0; i < 8; i++)
            {
                areChildrenHoleless &= UpdateQuadtreeClipmaps(node.Children[i], cameraPosition, minNodeSize, ref numRequestedFromCache, distanceFactor);
            }
            if (areChildrenHoleless) return true;
            // Going to try and use this lod level or one up the tree
            // First disable all children since not complete; Is this costly? I think this only happens in case suddenly a very low resolution mesh is needed, not sure how often that happens
            for (int i = 0; i < 8; i++)
                clearRenderInfo(node.Children[i]);

            return trySetRenderData(node, renderDataOrNull);

        }

        /// <summary>
        /// Walk the tree, request chunks that should be visible from the cache, and built a tree using what is available in the cache
        /// </summary>
        public bool UpdateQuadtreeClipmaps2(RenderOctreeNode node, Vector3 cameraPosition, int minNodeSize, ref int numRequestedFromCache, float distanceFactor = 1.2f)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + Vector3.one * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);


            if (node.DataNode == null) node.DataNode = getNode(node);

            // This node might be needed request it in cache
            VoxelChunkRenderer renderDataOrNull =  renderDataCache.Get(new NodeAndVersion(node, node.DataNode.VoxelData.LastChangeFrame));
            numRequestedFromCache++;


            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            if (dist > node.Size * distanceFactor)
            {
                // This is a valid node size at this distance, so remove all children
                helper.Merge(node);

                return trySetRenderData(node, renderDataOrNull);
            }
            else
            {
                if (node.Children == null)
                    helper.Split(node, false, minNodeSize);

                if (node.Children == null)
                {
                    // Minlevel
                    return trySetRenderData(node, renderDataOrNull);
                }
            }

            var areChildrenHoleless = true;
            for (int i = 0; i < 8; i++)
            {
                areChildrenHoleless &= UpdateQuadtreeClipmaps2(node.Children[i], cameraPosition, minNodeSize, ref numRequestedFromCache, distanceFactor);
            }
            //if (areChildrenHoleless) return true;
            // Going to try and use this lod level or one up the tree
            // First disable all children since not complete; Is this costly? I think this only happens in case suddenly a very low resolution mesh is needed, not sure how often that happens
            //for (int i = 0; i < 8; i++)
            //    clearRenderInfo(node.Children[i]);

            //return trySetRenderData(node, renderDataOrNull);
            return true;

        }

        private void clearRenderInfo(RenderOctreeNode node)
        {
            if (node.RenderObject != null)
            {
                node.RenderObject.gameObject.SetActive(false);
                node.RenderObject.transform.SetParent(ChunkCache);
            }
            node.RenderObject = null;

            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
                clearRenderInfo(node.Children[i]);
        }


        public bool trySetRenderData(RenderOctreeNode node, VoxelChunkRenderer renderDataOrNull)
        {
            return true;
            if (renderDataOrNull == null) return false; // Cant render

            if (node.RenderObject != null)
            {
                if (node.RenderObject == renderDataOrNull) return true; // Already ok
                node.RenderObject.gameObject.SetActive(false);
                node.RenderObject.transform.SetParent(ChunkCache);
            }

            // Warn this is comfusing because the unity component is actually an entire game object


            //if (node.Children != null)
            //{
            //    node.DestroyRenderObject(); // not a leaf
            //    return; // Only leafs
            //}
            //if (node.RenderObject != null) return; // already generated

            //var dataNode = VoxelWorld.GetNode(node.LowerLeft, node.Depth);

            //var renderObject = new GameObject();
            //var comp = renderObject.AddComponent<VoxelChunkRenderer>();
            var comp = renderDataOrNull;
            comp.MaterialsDictionary = materialsDictionary;
            //comp.SetChunk(dataNode.VoxelData);
            comp.SetWorldcoords(node.LowerLeft, node.Size / (float)(VoxelWorld.ChunkSize.X));// TOOD: DANGEROES

            comp.transform.SetParent(VisibleChunks);
            node.RenderObject = comp;
            comp.gameObject.SetActive(true);


            //node.SetRenderData(renderDataOrNull);
            return true;
        }

        private class NodeAndVersion
        {
            public RenderOctreeNode Node;
            public int ChangedFrame;

            public NodeAndVersion(RenderOctreeNode node, int changedFrame)
            {
                Node = node;
                ChangedFrame = changedFrame;
            }

            protected bool Equals(NodeAndVersion other)
            {
                return Equals(Node, other.Node) && ChangedFrame == other.ChangedFrame;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((NodeAndVersion)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Node != null ? Node.GetHashCode() : 0) * 397) ^ ChangedFrame;
                }
            }
        }

        private OctreeNode getNode(RenderOctreeNode f)
        {
            return VoxelWorld.GetNode(f.LowerLeft, f.Depth);
        }


        public void Update()
        {
            if (VoxelWorld == null) return;
            int numRequested = 0;
            Profiler.BeginSample("UpdateClipmaps");
            UpdateQuadtreeClipmaps2(root, Camera.main.transform.position, VoxelWorld.ChunkSize.X, ref numRequested, LODDistanceFactor);
            Profiler.EndSample();
            if (ShowOctree)
                helper.DrawLines(root, manager);


            Debug.Log("Requested: " + numRequested + " Missing: " + renderDataCache.GetMissingData().Count());

            var tasks = renderDataCache.GetMissingData()
                .Take(30)
                .Select(f => new Task
                {
                    node = f,
                    chunkData = getNode(f.Node).VoxelData.Data
                }).ToArray();

            workingQueue.Enqueue(tasks);
            Result result;
            Profiler.BeginSample("createRenderObject");
            while (resultsQueue.TryDequeue(out result))
            {
                var render = createRenderObject(result.node, result.data);

                renderDataCache.AddData(result.node, render);
            }
            Profiler.EndSample();

            //if (toRender != null)
            //{
            //    var dataNode = VoxelWorld.GetNode(toRender.LowerLeft, toRender.Depth);

            //    var data = VoxelChunkRenderer.generateMesh(meshGenerator, dataNode.VoxelData.Data);

            //    var render = createRenderObject(data);

            //    renderDataCache.AddData(toRender, render);
            //}

            helper.VisitTopDown(root, node =>
            {
                applyToRenderer(node);
                //createRenderObject(node);
            });

        }

        public void anderProcessorProgrammake()
        {
            Task[] tasks = new Task[0];
            int i = 0;
            while (stopped == 0)
            {
                Task[] nTasks;
                while (workingQueue.TryDequeue(out nTasks))
                {
                    i = 0;
                    tasks = nTasks;
                }
                if (tasks.Length <= i)
                {
                    Debug.Log("Idling");

                    Thread.Sleep(10);
                    continue;
                }
                var task = tasks[i];
                i++;

                var data = VoxelChunkRenderer.generateMesh(meshGenerator, task.chunkData); // DANGEROES multithreaded
                resultsQueue.Enqueue(new Result
                {
                    data = data,
                    node = task.node
                });

            }
        }


        private ConcurrentQueue<Task[]> workingQueue = new ConcurrentQueue<Task[]>();
        private ConcurrentQueue<Result> resultsQueue = new ConcurrentQueue<Result>();

        private struct Task
        {
            public NodeAndVersion node;
            public Array3D<VoxelData> chunkData;
        }
        private struct Result
        {
            public VoxelChunkRenderer.MeshData data;
            public NodeAndVersion node;
        }

        private void applyToRenderer(RenderOctreeNode node)
        {
            // Not sure if needed
        }

        private VoxelChunkRenderer createRenderObject(NodeAndVersion node, VoxelChunkRenderer.MeshData meshData)
        {
            var renderObject = new GameObject();
            renderObject.name = "Node " + node.ChangedFrame + " " + node.Node.LowerLeft + " " + node.Node.Size + " V: " + meshData.doubledVertices.Count;
            var comp = renderObject.AddComponent<VoxelChunkRenderer>();
            comp.AutomaticallyGenerateMesh = false;
            comp.MaterialsDictionary = materialsDictionary;
            comp.setMeshToUnity(meshData);
            comp.transform.SetParent(ChunkCache);

            renderObject.SetActive(false);
            return comp;


        }
    }
}
