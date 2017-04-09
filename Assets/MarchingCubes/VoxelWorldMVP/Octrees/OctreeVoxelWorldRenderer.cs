using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
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

        private AsyncLRUCache<RenderOctreeNode,object> renderDataCache;
        public int RenderCacheSize = 100;


        //private VoxelChunkMeshGenerator voxelChunkMeshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());

        public void Start()
        {
            renderDataCache = new AsyncLRUCache<RenderOctreeNode, object>(RenderCacheSize);
            
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
        public bool UpdateQuadtreeClipmaps(RenderOctreeNode node, Vector3 cameraPosition, int minNodeSize, float distanceFactor = 1.2f)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + Vector3.one * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            // This node might be needed request it in cache
            var renderDataOrNull = renderDataCache.Get(node);


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
                areChildrenHoleless &= UpdateQuadtreeClipmaps(node.Children[i], cameraPosition, minNodeSize, distanceFactor);
            }
            if (areChildrenHoleless) return true;
            // Going to try and use this lod level or one up the tree
            // First disable all children since not complete; Is this costly? I think this only happens in case suddenly a very low resolution mesh is needed, not sure how often that happens
            for (int i = 0; i < 8; i++)
                clearRenderInfo(node.Children[i]);

            return trySetRenderData(node, renderDataOrNull);

        }

        private void clearRenderInfo(RenderOctreeNode node)
        {
            throw new NotImplementedException();
            //node.SetRenderData(null);
            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
                clearRenderInfo(node.Children[i]);
        }


        public bool trySetRenderData(RenderOctreeNode node,object renderDataOrNull)
        {
            throw new NotImplementedException();

            if (renderDataOrNull == null) return false; // Cant render
            //node.SetRenderData(renderDataOrNull);
            return true;
        }



        public void Update()
        {
            if (VoxelWorld == null) return;
            helper.UpdateQuadtreeClipmaps(root, Camera.main.transform.position, VoxelWorld.ChunkSize.X, LODDistanceFactor);
            if (ShowOctree)
                helper.DrawLines(root, manager);

            helper.VisitTopDown(root, node =>
            {
                applyToRenderer(node);
                //createRenderObject(node);
            });

        }

        private void applyToRenderer(RenderOctreeNode node)
        {
            throw new NotImplementedException();
        }

        private void createRenderObject(RenderOctreeNode node)
        {
            if (node.Children != null)
            {
                node.DestroyRenderObject(); // not a leaf
                return; // Only leafs
            }
            if (node.RenderObject != null) return; // already generated

            var dataNode = VoxelWorld.GetNode(node.LowerLeft, node.Depth);

            var renderObject = new GameObject();
            var comp = renderObject.AddComponent<VoxelChunkRenderer>();
            comp.MaterialsDictionary = materialsDictionary;
            comp.SetChunk(dataNode.VoxelData);
            comp.SetWorldcoords(node.LowerLeft, node.Size / (float)(VoxelWorld.ChunkSize.X));// TOOD: DANGEROES

            comp.transform.SetParent(transform);
            node.RenderObject = comp;


        }
    }
}
