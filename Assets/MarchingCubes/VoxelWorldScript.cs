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
    public class VoxelWorldScript : MonoBehaviour
    {
        private UniformVoxelWorldRenderer worldRenderer;
        private VoxelWorldRaycaster raycaster;

        public List<Material> Materials = new List<Material>();
        public int Size = 3;

        private VoxelMaterial MaterialGreen = new VoxelWorldMVP.VoxelMaterial() { color = Color.green };
        private VoxelMaterial MaterialRed = new VoxelWorldMVP.VoxelMaterial() { color = Color.red };

        public void Start()
        {

            var world = new VoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            worldRenderer = new UniformVoxelWorldRenderer(world, transform);

            worldRenderer.createRenderers(new Point3(1, 1, 1) * Size, Materials.ToArray());


            tools.Add(KeyCode.Alpha0, new NullState());
            tools.Add(KeyCode.Alpha1, new PlaceSphereState(world, MaterialRed));

            activeState = tools[KeyCode.Alpha1];
            activeState.Start();

            raycaster = new VoxelWorldRaycaster();

        }

        private IState activeState;
        private Dictionary<KeyCode, IState> tools = new Dictionary<KeyCode, IState>();
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

    internal interface IState
    {
        void Stop();
        void Update(RaycastHit? raycast);
        void Start();
    }

    class PlaceSphereState : IState
    {
        private readonly VoxelWorld world;

        private float range = 3;

        private WorldEditTool tool = new WorldEditTool();

        private VoxelMaterial MaterialRed;

        public PlaceSphereState(VoxelWorld world, VoxelMaterial materialRed)
        {
            this.world = world;
            this.MaterialRed = materialRed;
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;

            var min = (raycast.Value.point - new Vector3(1, 1, 1) * range).ToFloored();
            var max = (raycast.Value.point + new Vector3(1, 1, 1) * range).ToCeiled();

            if (Input.GetMouseButtonDown(0))
            {
                world.ForChunksInRange(min, max, (p, c) =>
                {
                    var offset = p.Multiply(world.ChunkSize);
                    var localHit = raycast.Value.point - offset;

                    tool.addSphere(c, localHit, range, MaterialRed);
                    c.LastChangeFrame = Time.frameCount;

                });

               
            }
            else if (Input.GetMouseButtonDown(1))
            {
                world.ForChunksInRange(min, max, (p, c) =>
                {
                    var offset = p.Multiply(world.ChunkSize);
                    var localHit = raycast.Value.point - offset;

                    tool.removeSphere(c, localHit, range, MaterialRed);
                    c.LastChangeFrame = Time.frameCount;

                });
            }


         
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
