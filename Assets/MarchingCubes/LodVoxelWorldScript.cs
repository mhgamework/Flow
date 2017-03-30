using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DualContouring;
using UnityEditor;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Unity component to bootstrap the sky islands voxel renderer
    /// </summary>
    public class LodVoxelWorldScript : MonoBehaviour
    {
        private UniformVoxelWorldRenderer worldRenderer;
        private VoxelWorldRaycaster raycaster;

        public List<Material> Materials = new List<Material>();
        public int Size = 3;

        private VoxelMaterial MaterialGreen = new VoxelWorldMVP.VoxelMaterial() { color = Color.green };
        private VoxelMaterial MaterialRed = new VoxelWorldMVP.VoxelMaterial() { color = Color.red };
        private VoxelMaterial MaterialBlue = new VoxelWorldMVP.VoxelMaterial() { color = Color.blue };



        public List<VoxelMaterial> VoxelMaterials;
        public VoxelMaterial ActiveMaterial;
        public float ActiveSize = 3;

        public void Start()
        {
            //VoxelWorld = new OctreeVoxelWorld(new DelegateVoxelWorldGenerator(sinWorld), minNodeSize, WorldDepth);
            //var world = new OctreeVoxelWorld(new ConstantVoxelWorldGenerator(int.MaxValue, null), minNodeSize, WorldDepth);
            var world = new OctreeVoxelWorld(new DelegateVoxelWorldGenerator(FlatWorldFunction), minNodeSize, WorldDepth);
            
            GetComponent<OctreeVoxelWorldRenderer>().Init(world);
            GetComponent<VoxelWorldEditorScript>().Init(world);
           
            //var world = new UniformVoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            //worldRenderer = new UniformVoxelWorldRenderer(world, transform);

            //worldRenderer.createRenderers(new Point3(1, 1, 1) * Size, Materials.ToArray());

            //ActiveMaterial = MaterialGreen;
            VoxelMaterials = new List<VoxelMaterial>(new[] { MaterialGreen, MaterialRed, MaterialBlue });


            //raycaster = new VoxelWorldRaycaster();

        }

        private int minNodeSize = 16;
        public int WorldDepth = 3;
        private VoxelData LowerleftSphereWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = SdfFunctions.Sphere(arg1, new Vector3(0, 0, 0), minNodeSize * 3),
                Material = MaterialGreen

            };
        }
        private VoxelData FlatWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = arg1.y-10,
                Material = MaterialGreen

            };
        }

        private VoxelData sinWorld(Vector3 arg1, int samplingInterval)
        {
            double dens = (Math.Sin(arg1.x * 0.01) + Math.Cos(arg1.z * 0.01)) * 50 - arg1.y + 3000;
            dens += (Math.Sin(arg1.x * 0.11) + Math.Cos(arg1.z * 0.09)) * 4.3;

            //dens += (Math.Sin(arg1.x * 0.002) + Math.Cos(arg1.z * 0.0019)) * 651;
            //dens += (Math.Sin(arg1.x * 0.00021) + Math.Cos(arg1.z * 0.0002)) * 3000;
            return new VoxelData() { Density = (float)dens, Material = MaterialGreen };
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

       
    }
}
