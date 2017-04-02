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
        public Material[] Materials;
        private VoxelMaterial MaterialGreen = new VoxelWorldMVP.VoxelMaterial() { color = Color.green };
        private VoxelMaterial MaterialRed = new VoxelWorldMVP.VoxelMaterial() { color = Color.red };
        private VoxelMaterial MaterialBlue = new VoxelWorldMVP.VoxelMaterial() { color = Color.blue };

        public bool ShowOctree = false;


        private Dictionary<Point3, VoxelChunkRenderer> chunks = new Dictionary<Point3, VoxelChunkRenderer>();

        public Transform Container;

        // Set by some script
        public OctreeVoxelWorld VoxelWorld;

        private RenderOctreeNode root;
        private ClipMapsOctree<RenderOctreeNode> helper = new ClipMapsOctree<RenderOctreeNode>();
        private LineManager3D manager = new LineManager3D();
     

        //private VoxelChunkMeshGenerator voxelChunkMeshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());

        public void Start()
        {
         
        }

        public void Init(OctreeVoxelWorld world)
        {
            VoxelWorld = world;
            root = helper.Create(VoxelWorld.Root.Size, VoxelWorld.Root.LowerLeft);
            
        }

     

     

        public void Update()
        {
            if (VoxelWorld == null) return;
            helper.UpdateQuadtreeClipmaps(root, Camera.main.transform.position, VoxelWorld.ChunkSize.X);
            if (ShowOctree)
                helper.DrawLines(root, manager);

            helper.VisitTopDown(root, node =>
            {
                createRenderObject(node);
            });

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
            comp.Materials = Materials;
            comp.SetChunk(dataNode.VoxelData);
            comp.SetWorldcoords(node.LowerLeft, node.Size / 16.0f);// TOOD: DANGEROES

            comp.transform.SetParent(transform);
            node.RenderObject = comp;


        }
    }
}
