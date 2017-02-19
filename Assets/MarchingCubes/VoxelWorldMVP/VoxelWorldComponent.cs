using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Unity component to bootstrap the sky islands voxel renderer
    /// </summary>
    public class VoxelWorldComponent : MonoBehaviour
    {
        public Material VoxelMaterial;
        private VoxelWorldRenderer worldRenderer;
        

        public void Start()
        {
            var world = new VoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            worldRenderer = new VoxelWorldRenderer(world, transform);
            
            worldRenderer.createRenderers(new Point3(1, 1, 1)*10,VoxelMaterial);

        }

        private float worldFunction(Vector3 arg1)
        {
            var repeat = 20;
            var radius =7;

            var c = new Vector3(1,1,1)*repeat;
            var q = mod(arg1, c) - 0.5f * c;
            var s = q.magnitude - radius;
            return s;//arg1.x+arg1.z-10+arg1.y*0.1f;
        }

        private Vector3 mod(Vector3 p, Vector3 c)
        {
            return new Vector3(p.x % c.x, p.y % c.y, p.z % c.z);
        }
    }
}
