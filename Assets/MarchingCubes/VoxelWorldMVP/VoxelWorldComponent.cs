using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelWorldComponent : MonoBehaviour
    {
        private VoxelWorldRenderer worldRenderer;

        public void Start()
        {
            var world = new VoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            worldRenderer = new VoxelWorldRenderer(world, transform);
            worldRenderer.createRenderers(new Point3(1, 1, 1)*20);

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

    public class DelegateVoxelWorldGenerator : IWorldGenerator
    {
        private readonly Func<Vector3, float> worldFunction;

        public DelegateVoxelWorldGenerator(Func<Vector3, float> worldFunction)
        {
            this.worldFunction = worldFunction;
        }

        public UniformVoxelData Generate(Point3 start, Point3 chunkSize)
        {
            var ret = new UniformVoxelData();
            ret.Data = new Array3D<float>(chunkSize);
            ret.Data.ForEach((v, p) =>
            {
                ret.Data[p] = worldFunction(p + start);
            });

            return ret;
        }
    }
}
