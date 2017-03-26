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
        UniformVoxelWorld world;

        private Dictionary<Point3, VoxelChunkRenderer> chunks = new Dictionary<Point3, VoxelChunkRenderer>();

        public Transform Container;

        // Set by some script
        public OctreeVoxelWorld VoxelWorld;

        private RenderOctreeNode root;
        private ClipMapsOctree<RenderOctreeNode> helper = new ClipMapsOctree<RenderOctreeNode>();
        private LineManager3D manager = new LineManager3D();

        public void Start()
        {
            VoxelWorld = new OctreeVoxelWorld(new DelegateVoxelWorldGenerator(emptyWorldFunction), 16, 2);
            root = helper.Create(VoxelWorld.Root.Size, VoxelWorld.Root.LowerLeft);
        }

        private VoxelData emptyWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = 1,
                Material = null

            };
        }

        //public void createRenderers(Point3 chunksToRender,Material[] voxelMaterial)
        //{
        //    Point3.ForEach(chunksToRender, p =>
        //     {
        //         var obj = new GameObject();
        //         obj.transform.SetParent(container);
        //         var subRenderer = obj.AddComponent<VoxelChunkRenderer>();
        //         subRenderer.Materials = voxelMaterial;
        //         //subRenderer.VoxelMaterial = voxelMaterial;
        //         subRenderer.SetChunk(world.GetChunk(p));
        //         subRenderer.SetWorldcoords(p, world.ChunkSize);
        //         chunks.Add(p, subRenderer);


        //     });
        //}

        public void Update()
        {
            helper.UpdateQuadtreeClipmaps(root, Camera.main.transform.position, 16);
            helper.DrawLines(root, manager);
        }


    }
}
