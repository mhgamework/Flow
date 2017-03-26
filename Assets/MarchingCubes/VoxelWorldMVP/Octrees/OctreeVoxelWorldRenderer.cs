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

        UniformVoxelWorld world;

        private Dictionary<Point3, VoxelChunkRenderer> chunks = new Dictionary<Point3, VoxelChunkRenderer>();

        public Transform Container;

        // Set by some script
        public OctreeVoxelWorld VoxelWorld;

        private RenderOctreeNode root;
        private ClipMapsOctree<RenderOctreeNode> helper = new ClipMapsOctree<RenderOctreeNode>();
        private LineManager3D manager = new LineManager3D();
        private int minNodeSize = 16;
        public int WorldDepth = 3;

        //private VoxelChunkMeshGenerator voxelChunkMeshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());

        public void Start()
        {
            VoxelWorld = new OctreeVoxelWorld(new DelegateVoxelWorldGenerator(sinWorld), minNodeSize, WorldDepth);
            root = helper.Create(VoxelWorld.Root.Size, VoxelWorld.Root.LowerLeft);
        }

        private VoxelData LowerleftSphereWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = SdfFunctions.Sphere(arg1, new Vector3(0, 0, 0), minNodeSize * 3),
                Material = MaterialGreen

            };
        }

        private VoxelData sinWorld(Vector3 arg1, int samplingInterval)
        {
            var dens = (Mathf.Sin(arg1.x * 0.01f) + Mathf.Cos(arg1.z * 0.01f)) * 50 - arg1.y + 100;
            dens += (Mathf.Sin(arg1.x * 0.11f) + Mathf.Cos(arg1.z * 0.09f)) * 4.3f;
            return new VoxelData() { Density = dens, Material = MaterialGreen };
        }
        private VoxelData worldFunction(Vector3 arg1, int samplingInterval)
        {
            if (samplingInterval > 5)
            {
                return new VoxelData() { Density = 1, Material = null };

            }
            var repeat = 20;
            var radius = 7;

            var c = new Vector3(1, 1, 1) * repeat;
            var q = mod(arg1, c) - 0.5f * c;
            var s = q.magnitude - radius;
            return new VoxelData() { Density = s, Material = MaterialGreen };
        }

        private Vector3 mod(Vector3 p, Vector3 c)
        {
            return new Vector3(p.x % c.x, p.y % c.y, p.z % c.z);
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
            helper.UpdateQuadtreeClipmaps(root, Camera.main.transform.position, minNodeSize);
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
