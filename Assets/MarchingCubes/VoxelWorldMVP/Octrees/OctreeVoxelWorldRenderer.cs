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

        public void Start()
        {

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
            List<RenderOctreeNode> outMissingRenderDataNodes, float distanceFactor = 1.2f, bool parentDirty = true)
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
            var hasRenderable = node.RenderObject != null && node.LastRenderFrame == getNode(node).VoxelData.LastChangeFrame;
            if (hasRenderable == node.ShouldRender) return false; // Done, unchanged
            outMissingRenderDataNodes.Add(node);
            if (parentDirty) return false;

            outDirtyNodes.Add(node); // Needs collapse
            return true;
        }

        List<RenderOctreeNode> outDirtyNodes = new List<RenderOctreeNode>();
        List<RenderOctreeNode> outMissingRenderdataNodes = new List<RenderOctreeNode>();
        private Dictionary<OctreeNode, VoxelChunkRenderer.MeshData> cache = new Dictionary<OctreeNode, VoxelChunkRenderer.MeshData>();
        public void Update()
        {
            if (VoxelWorld == null) return;

            UpdateClipmaps(root, Camera.main.transform.position, VoxelWorld.ChunkSize.X, outDirtyNodes,
                outMissingRenderdataNodes, LODDistanceFactor);

            // TODO async scheduling and processing, copy from lru

            foreach (var dirty in outDirtyNodes)
                tryCleanup(dirty);


            //UpdateQuadtreeClipmaps(root, Camera.main.transform.position, VoxelWorld.ChunkSize.X, LODDistanceFactor);
            if (ShowOctree)
                helper.DrawLines(root, manager);

            //helper.VisitTopDown(root, node =>
            //{
            //    createRenderObject(node);
            //});

        }

        private void tryCleanup(RenderOctreeNode dirty)
        {
            // still not ok
            var hasRenderables = checkLeafsReady(dirty);
            if (!hasRenderables) return;
            removeRenderable(dirty);
            activateRenderables(dirty);
        }

        private bool checkLeafsReady(RenderOctreeNode node)
        {

            if (node.ShouldRender) return hasRenderable(node);
            if (node.Children == null) return true;
            var ret = true;
            for (int i = 0; i < 8; i++)
            {
                ret &= checkLeafsReady(node.Children[i]);
            }
            return ret;
        }
        public void removeRenderable(RenderOctreeNode node)
        {
            node.RenderObject.gameObject.SetActive(false);
            node.RenderObject = null;
        }
        public void activateRenderables(RenderOctreeNode node)
        {
            if (node.ShouldRender)
            {
                if (node.RenderObject != null)
                    throw new InvalidOperationException(
                        "Not possible, if the parent is dirty and the holeless invariant holds, this should be null");
                var mesh = cache[getNode(node)];
                node.RenderObject = createREnderDAta(node, mesh);
                activateRenderdata(node, node.RenderObject);
            }
        }
        public bool hasRenderable(RenderOctreeNode node)
        {
            return cache.ContainsKey(getNode(node));
        }
        public void activateRenderdata(RenderOctreeNode node, VoxelChunkRenderer renderDataOrNull)
        {
            var comp = renderDataOrNull;
            comp.MaterialsDictionary = materialsDictionary;
            comp.SetWorldcoords(node.LowerLeft, node.Size / (float)(VoxelWorld.ChunkSize.X));// TOOD: DANGEROES

            comp.transform.SetParent(transform);
            comp.gameObject.SetActive(true);
        }
        private VoxelChunkRenderer createREnderDAta(RenderOctreeNode node, VoxelChunkRenderer.MeshData meshData)
        {
            var renderObject = new GameObject();
            renderObject.name = "Node " + node.LastRenderFrame + " " + node.LowerLeft + " " + node.Size + " V: " + meshData.doubledVertices.Count;
            var comp = renderObject.AddComponent<VoxelChunkRenderer>();
            comp.AutomaticallyGenerateMesh = false;
            comp.MaterialsDictionary = materialsDictionary;
            comp.setMeshToUnity(meshData);
            comp.transform.SetParent(transform);

            renderObject.SetActive(false);
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
            public RenderOctreeNode node;
            public Array3D<VoxelData> chunkData;
        }
        private struct Result
        {
            public VoxelChunkRenderer.MeshData data;
            public RenderOctreeNode node;
        }


    }
}
