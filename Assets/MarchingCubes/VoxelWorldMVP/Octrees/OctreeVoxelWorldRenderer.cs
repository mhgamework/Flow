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

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Holds state for renderers of the voxel world
    /// Responsible for updating the renderers of the voxel world
    /// 
    /// This IS a unity component, to create some confusion!
    /// Probably makes more sense
    /// </summary>
    public class OctreeVoxelWorldRenderer : MonoBehaviour
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

        private VoxelChunkMeshGenerator meshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());
        private int stopped;
        //private VoxelChunkMeshGenerator voxelChunkMeshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());
        private Thread t;

        public void Start()
        {
            t = new Thread(anderProcessorProgrammake);
            stopped = 0;
            t.Start();
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



        public void UpdateClipmaps(RenderOctreeNode node, Vector3 cameraPosition, int minNodeSize, List<RenderOctreeNode> outDirtyNodes,
            List<RenderOctreeNode> outMissingRenderDataNodes, float distanceFactor = 1.2f, bool parentDirty = false)
        {
            if (isQualityGoodEnough(node, cameraPosition, distanceFactor))
            {
                node.ShouldRender = true;
                checkDirty(node, outDirtyNodes, outMissingRenderdataNodes, parentDirty);
                return;
            }

            if (node.Children == null)
                helper.Split(node, false, minNodeSize);

            if (node.Children == null)
            {
                // Minlevel
                node.ShouldRender = true;
                checkDirty(node, outDirtyNodes, outMissingRenderdataNodes, parentDirty);
                return;
            }

            // Should not render
            node.ShouldRender = false;
            parentDirty = checkDirty(node, outDirtyNodes, outMissingRenderdataNodes, parentDirty);

            for (int i = 0; i < 8; i++)
                UpdateClipmaps(node.Children[i], cameraPosition, minNodeSize, outDirtyNodes, outMissingRenderDataNodes, distanceFactor, parentDirty);
        }

        private static bool isQualityGoodEnough(RenderOctreeNode node, Vector3 cameraPosition, float distanceFactor)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + Vector3.one * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            var qualityGoodEnough = dist > node.Size * distanceFactor;
            return qualityGoodEnough;
        }

        public bool checkDirty(RenderOctreeNode node, List<RenderOctreeNode> outDirtyNodes, List<RenderOctreeNode> outMissingRenderDataNodes, bool parentDirty)
        {
            var hasRenderable = hasCorrectRenderable(node);
            if (hasRenderable == node.ShouldRender) return false; // Done, unchanged
            outMissingRenderDataNodes.Add(node);
            if (parentDirty) return false;

            outDirtyNodes.Add(node); // Needs collapse
            return true;
        }

        private bool hasCorrectRenderable(RenderOctreeNode node)
        {
            return node.RenderObject != null && node.LastRenderFrame == getNode(node).VoxelData.LastChangeFrame;
        }

        List<RenderOctreeNode> outDirtyNodes = new List<RenderOctreeNode>();
        List<RenderOctreeNode> outMissingRenderdataNodes = new List<RenderOctreeNode>();
        private Dictionary<OctreeNode, Result> cache = new Dictionary<OctreeNode, Result>();
        public void Update()
        {
            if (VoxelWorld == null) return;

            outDirtyNodes.Clear();
            outMissingRenderdataNodes.Clear();

            UpdateClipmaps(root, Camera.main.transform.position, VoxelWorld.ChunkSize.X, outDirtyNodes,
                outMissingRenderdataNodes, LODDistanceFactor);

            Debug.Log("Dirty: " + outDirtyNodes.Count + " Missing: " + outMissingRenderdataNodes.Count + " Cache: " + cache.Count);

            processAsyncMessages(outMissingRenderdataNodes.Where(n => !cache.ContainsKey(getNode(n))).ToArray());

            // subtle invariant, if a node is in the cache, it will not be in the async working queue. only way node is missprocessed is when it is in process while sending new requests which should be fine

            foreach (var dirty in outDirtyNodes)
                if (checkAllRenderablesInCache(dirty))
                    transìtion(dirty);


            //UpdateQuadtreeClipmaps(root, Camera.main.transform.position, VoxelWorld.ChunkSize.X, LODDistanceFactor);
            if (ShowOctree)
                helper.DrawLines(root, manager);

            //helper.VisitTopDown(root, node =>
            //{
            //    createRenderObject(node);
            //});

        }

        private void processAsyncMessages(RenderOctreeNode[] newTaskList)
        {
            var tasks = newTaskList.Select(f => new Task
            {
                node = f,
                Frame = getNode(f).VoxelData.LastChangeFrame,
                chunkData = getNode(f).VoxelData.Data
            }).ToArray();

            workingQueue.Enqueue(tasks);
            Result result;
            while (resultsQueue.TryDequeue(out result))
            {
                //var render = createRenderObject(result.node, result.data);

                cache[getNode(result.node)] = result;
            }
        }

        private void transìtion(RenderOctreeNode node)
        {
            if (!node.ShouldRender && node.RenderObject != null)
                removeRenderable(node);
            if (node.ShouldRender && !hasCorrectRenderable(node))
            {
                removeRenderable(node);
                activateRenderable(node);
            }

            if (node.Children == null) return;

            for (int i = 0; i < 8; i++)
                transìtion(node.Children[i]);

            if (node.ShouldRender)
                helper.Merge(node);
        }

        private bool checkAllRenderablesInCache(RenderOctreeNode node)
        {

            if (node.ShouldRender) return cache.ContainsKey(getNode(node));
            if (node.Children == null) return true;
            var ret = true;
            for (int i = 0; i < 8; i++)
            {
                ret &= checkAllRenderablesInCache(node.Children[i]);
            }
            return ret;
        }

        public void removeWrongRenderables(RenderOctreeNode node)
        {
            if (!node.ShouldRender)
            {
                if (node.RenderObject != null)
                    removeRenderable(node);
            }
            if (node.Children == null) return;

        }

        public void removeRenderable(RenderOctreeNode node)
        {
            if (node.RenderObject != null)
            {
                node.RenderObject.gameObject.SetActive(false);
                Destroy(node.RenderObject.gameObject);
            }
            node.RenderObject = null;
        }
        public void activateRenderable(RenderOctreeNode node)
        {
            if (node.RenderObject != null)
                throw new InvalidOperationException();
            var result = cache[getNode(node)];
            cache.Remove(getNode(node));
            node.RenderObject = createREnderDAta(node, result);
            node.LastRenderFrame = result.Frame;
            activateRenderdata(node, node.RenderObject);
        }

        public void activateRenderdata(RenderOctreeNode node, VoxelChunkRenderer renderDataOrNull)
        {
            var comp = renderDataOrNull;
            comp.MaterialsDictionary = materialsDictionary;
            comp.SetWorldcoords(node.LowerLeft, node.Size / (float)(VoxelWorld.ChunkSize.X));// TOOD: DANGEROES

            comp.transform.SetParent(transform);
            //comp.gameObject.SetActive(true);
        }
        private VoxelChunkRenderer createREnderDAta(RenderOctreeNode node, Result result)
        {
            var renderObject = new GameObject();
            renderObject.name = "Node " + result.Frame + " " + node.LowerLeft + " " + node.Size + " V: " + result.data.doubledVertices.Count;
            var comp = renderObject.AddComponent<VoxelChunkRenderer>();
            comp.AutomaticallyGenerateMesh = false;
            comp.MaterialsDictionary = materialsDictionary;
            comp.setMeshToUnity(result.data);
            comp.transform.SetParent(transform);

            //renderObject.SetActive(false);
            return comp;


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
            if (f.DataNode == null)
                f.DataNode = VoxelWorld.GetNode(f.LowerLeft, f.Depth);
            return f.DataNode;
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
                    //Debug.Log("Idling");

                    Thread.Sleep(10);
                    continue;
                }
                var task = tasks[i];
                i++;


                var data = VoxelChunkRenderer.generateMesh(meshGenerator, task.chunkData); // DANGEROES multithreaded
                resultsQueue.Enqueue(new Result
                {
                    data = data,
                    Frame = task.Frame,
                    node = task.node
                });

            }
        }


        private ConcurrentQueue<Task[]> workingQueue = new ConcurrentQueue<Task[]>();
        private ConcurrentQueue<Result> resultsQueue = new ConcurrentQueue<Result>();

        private struct Task
        {
            public RenderOctreeNode node;
            public int Frame;
            public Array3D<VoxelData> chunkData;
        }
        private struct Result
        {
            public VoxelChunkRenderer.MeshData data;
            public int Frame;
            public RenderOctreeNode node;
        }

    }
}
