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
    public class VoxelWorldComponent : MonoBehaviour
    {
        public Material VoxelMaterial;
        private VoxelWorldRenderer worldRenderer;
        private VoxelWorldRaycaster raycaster;


        public void Start()
        {

            var world = new VoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            worldRenderer = new VoxelWorldRenderer(world, transform);

            worldRenderer.createRenderers(new Point3(1, 1, 1) * 5, VoxelMaterial);


            tools.Add(KeyCode.Alpha0, new NullState());
            tools.Add(KeyCode.Alpha1, new PlaceSphereState(world));

            activeState = tools[KeyCode.Alpha0];
            activeState.Start();

        }

        private IState activeState;
        private Dictionary<KeyCode, IState> tools;
        public void Update()
        {
            foreach (var k in tools)
            {
                if (!Input.GetKeyDown(k.Key)) continue;
                activeState.Stop();
                activeState = k.Value;
                activeState.Start();
                break;

            }

            activeState.Update(raycaster.raycast());
        }


        private float worldFunction(Vector3 arg1)
        {
            var repeat = 20;
            var radius = 7;

            var c = new Vector3(1, 1, 1) * repeat;
            var q = mod(arg1, c) - 0.5f * c;
            var s = q.magnitude - radius;
            return s;//arg1.x+arg1.z-10+arg1.y*0.1f;
        }

        private Vector3 mod(Vector3 p, Vector3 c)
        {
            return new Vector3(p.x % c.x, p.y % c.y, p.z % c.z);
        }
    }

    internal interface IState
    {
        void Stop();
        void Update(RaycastHit? raycast);
        void Start();
    }

    class PlaceSphereState : IState
    {
        private readonly VoxelWorld world;

        private float range = 5;

        public PlaceSphereState(VoxelWorld world)
        {
            this.world = world;
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;
            if (!Input.GetMouseButton(0)) return;

            var min = (raycast.Value.point - new Vector3(1, 1, 1) * range).ToFloored();
            var max = (raycast.Value.point + new Vector3(1, 1, 1) * range).ToCeiled();

            
            world.ForChunksInRange(min, max, (p, c) =>
            {
                var offset = p.Multiply(world.ChunkSize);
                var localHit = raycast.Value.point - offset;

                throw new System.Exception();
                //c.Data.ForEach((val, pos) =>
                //{
                //    c.Data[pos] = Mathf.Max(val, range - (pos - localHit).magnitude);
                //});
                //c.LastChangeFrame = Time.frameCount;

            });
        }

        public void Start()
        {
        }
    }

    class NullState : IState
    {
        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
        }

        public void Start()
        {
        }
    }
}
