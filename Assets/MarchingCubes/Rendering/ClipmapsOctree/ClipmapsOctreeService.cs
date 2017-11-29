using System;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.Rendering.AsyncCPURenderer;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.VR;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Graphics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.Rendering.ClipmapsOctree
{
    /// <summary>
    /// Algorithm to determine which size voxel chunks to render given the camera position
    /// Chunks get bigger when further from the camera
    /// The algorithm works by swapping out subtrees as a whole, to never have holes when transitioning.
    /// It requests asynchronously chunks that it needs, and when enough chunks are available to make a 
    /// hole-less transition it swappes the chunks.
    /// </summary>
    public class ClipmapsOctreeService
    {

        //public bool ShowOctree = false;

        //public Transform Container;

        // Set by some script
        public OctreeVoxelWorld VoxelWorld;
        //public VoxelChunkRendererPoolScript ChunkPool;

        private RenderOctreeNode root;
        private ClipMapsOctree<RenderOctreeNode> helper;
        //private LineManager3D manager = new LineManager3D();

        public float LODDistanceFactor = 1.2f;


        private IVoxelRenderer rendererService;


        //private ConcurrentVoxelGenerator concurrentVoxelGenerator = new ConcurrentVoxelGenerator();

        List<RenderOctreeNode> outDirtyNodes = new List<RenderOctreeNode>();
        List<RenderOctreeNode> outMissingRenderdataNodes = new List<RenderOctreeNode>();
        private List<ChunkCoord> tempTaskList = new List<ChunkCoord>();


        public ClipmapsOctreeService(OctreeVoxelWorld voxelWorld, IVoxelRenderer rendererService)
        {
            VoxelWorld = voxelWorld;
            this.rendererService = rendererService;

            helper = new ClipMapsOctree<RenderOctreeNode>(new RenderOctreeNodeFactory(this));

            root = helper.Create(VoxelWorld.Root.Size, VoxelWorld.Root.LowerLeft);

            //this.materialsDictionary = voxelMaterials.ToDictionary(v => v.color, c =>
            //{
            //    var mat = new Material(TemplateMaterial);
            //    mat.color = c.color;
            //    return mat;
            //});
        }


        /// <summary>
        /// Camera position in voxel space
        /// </summary>
        /// <param name="cameraPosition"></param>
        public void UpdateRendererState(Vector3 cameraPosition)
        {
            if (VoxelWorld == null) return;

            Profiler.BeginSample("PRF-ClearBuffers");

            outDirtyNodes.Clear();
            outMissingRenderdataNodes.Clear();

            Profiler.EndSample();

            Profiler.BeginSample("PRF-UpdateClipmaps");

            UpdateClipmaps(root,
                cameraPosition,
                VoxelWorld.ChunkSize.X,
                outDirtyNodes,
                outMissingRenderdataNodes,
                LODDistanceFactor);

            Profiler.EndSample();

            //debugText.SetText("Tasks: ", outMissingRenderdataNodes.Count.ToString());
            Profiler.BeginSample("PRF-Async");

            tempTaskList.Clear(); // I added this no clue why this wasnt here
            for (var i = 0; i < outMissingRenderdataNodes.Count; i++)
            {
                var f = outMissingRenderdataNodes[i];
                tempTaskList.Add(toCoord(f));
            }
            rendererService.PrepareShowChunk(tempTaskList);

            Profiler.EndSample();

            // subtle invariant, if a node is in the cache, it will not be in the async working queue. only way node is missprocessed is when it is in process while sending new requests which should be fine
            Profiler.BeginSample("PRF-Transition");

            foreach (var dirty in outDirtyNodes)
                if (checkAllRenderablesAvailable(dirty))
                    transìtion(dirty);

            Profiler.EndSample();




        }


        public class RenderOctreeNodeFactory : ClipMapsOctree<RenderOctreeNode>.EmptyConstructorFactory
        {
            private ClipmapsOctreeService clipmapsOctreeService;

            public RenderOctreeNodeFactory(ClipmapsOctreeService clipmapsOctreeService)
            {
                this.clipmapsOctreeService = clipmapsOctreeService;
            }

            public override RenderOctreeNode Create(RenderOctreeNode parent, int size, int depth, Point3 pos)
            {
                var ret = base.Create(parent, size, depth, pos);
                ret.clipmapsOctreeService = clipmapsOctreeService;
                return ret;
            }
        }


        /// <summary>
        /// OutDirtyNodes contain the highest level nodes that have a active rendering state inconsistent
        /// with the desired state. All lower level nodes that are inconsistent are contained in the parent one.
        /// 
        /// The transition method will take one of these nodes and update it and all children to a new holeless state 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cameraPosition"></param>
        /// <param name="minNodeSize"></param>
        /// <param name="outDirtyNodes"></param>
        /// <param name="outMissingRenderDataNodes"></param>
        /// <param name="distanceFactor"></param>
        /// <param name="parentDirty"></param>
        public void UpdateClipmaps(
            RenderOctreeNode node,
            Vector3 cameraPosition,
            int minNodeSize,
            List<RenderOctreeNode> outDirtyNodes,
            List<RenderOctreeNode> outMissingRenderDataNodes,
            float distanceFactor = 1.2f,
            bool parentDirty = false)
        {
            if (isQualityGoodEnough(node, cameraPosition, distanceFactor))
            {
                node.ShouldRender = true;
                checkDirty(node, outDirtyNodes, outMissingRenderdataNodes, parentDirty);
                if (node.Children != null)
                    for (int i = 0; i < 8; i++)
                        setShouldRenderFalseRecursive(node.Children[i]);
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
                UpdateClipmaps(node.Children[i], cameraPosition, minNodeSize, outDirtyNodes, outMissingRenderDataNodes,
                    distanceFactor, parentDirty);
        }

        private void setShouldRenderFalseRecursive(RenderOctreeNode node)
        {
            node.ShouldRender = false;
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
                setShouldRenderFalseRecursive(node.Children[i]);
        }

        private static bool isQualityGoodEnough(RenderOctreeNode node, Vector3 cameraPosition, float distanceFactor)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + Vector3.one * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            var qualityGoodEnough = dist > node.Size * distanceFactor;
            return qualityGoodEnough;
        }

        public bool checkDirty(RenderOctreeNode node, List<RenderOctreeNode> outDirtyNodes,
            List<RenderOctreeNode> outMissingRenderDataNodes, bool parentDirty)
        {
            // Not dirty when
            if (node.ShouldRender && hasUpToDateRenderable(node)) return false;
            if (!node.ShouldRender && hasNoRenderable(node)) return false;

            // Dirty!
            if (node.ShouldRender) outMissingRenderDataNodes.Add(node);

            if (!parentDirty)
                outDirtyNodes.Add(node); // root-est dirty node

            return true;
        }

        private static bool hasNoRenderable(RenderOctreeNode node)
        {
            return node.RenderObject == null;
        }

        private bool hasUpToDateRenderable(RenderOctreeNode node)
        {
            return node.RenderObject != null && node.LastRenderFrame == rendererService.GetLastChangeFrame(toCoord(node));
        }


        private void transìtion(RenderOctreeNode node)
        {
            if (!node.ShouldRender && node.RenderObject != null)
                removeRenderable(node);
            if (node.ShouldRender && !hasUpToDateRenderable(node))
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

        private bool checkAllRenderablesAvailable(RenderOctreeNode node)
        {
            if (node.ShouldRender) return rendererService.CanShowChunk(toCoord(node)) || hasUpToDateRenderable(node);
            if (node.Children == null) return true;
            var ret = true;
            for (int i = 0; i < 8; i++)
            {
                ret &= checkAllRenderablesAvailable(node.Children[i]);
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
            DestroyRenderObject(node);
        }

        public void activateRenderable(RenderOctreeNode node)
        {
            if (node.RenderObject != null)
                throw new InvalidOperationException();
            if (!rendererService.CanShowChunk(toCoord(node)))
            {
                throw new InvalidOperationException();
            }
            int frame;
            var result = rendererService.ShowChunk(toCoord(node), out frame);


            node.RenderObject = result;
            node.LastRenderFrame = frame;
        }

        private ChunkCoord toCoord(RenderOctreeNode node)
        {
            return new ChunkCoord(node.LowerLeft, node.Depth);
        }


    
        public void OnDestroyRenderNode(RenderOctreeNode renderOctreeNode)
        {
            DestroyRenderObject(renderOctreeNode);
        }

        private void DestroyRenderObject(RenderOctreeNode renderOctreeNode)
        {
            if (renderOctreeNode.RenderObject != null)
            {
                rendererService.HideChunk(renderOctreeNode.RenderObject);
                //RenderObject.gameObject.transform.SetParent(null);
                //GameObject.Destroy(RenderObject.gameObject);
            }
            renderOctreeNode.RenderObject = null;
        }
    }
}