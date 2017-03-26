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

            //var world = new UniformVoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            //worldRenderer = new UniformVoxelWorldRenderer(world, transform);

            //worldRenderer.createRenderers(new Point3(1, 1, 1) * Size, Materials.ToArray());

            //ActiveMaterial = MaterialGreen;
            VoxelMaterials = new List<VoxelMaterial>(new[] { MaterialGreen, MaterialRed, MaterialBlue });


            //raycaster = new VoxelWorldRaycaster();

        }

        private VoxelData worldFunction(Vector3 arg1)
        {
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
