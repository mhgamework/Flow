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
    public class VoxelWorldScript : MonoBehaviour
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

            var world = new VoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            worldRenderer = new UniformVoxelWorldRenderer(world, transform);

            worldRenderer.createRenderers(new Point3(1, 1, 1) * Size, Materials.ToArray());

            ActiveMaterial = MaterialGreen;
            VoxelMaterials = new List<VoxelMaterial>(new[] { MaterialGreen, MaterialRed, MaterialBlue });

            tools.Add(KeyCode.Alpha0, new NullState());
            tools.Add(KeyCode.Alpha1, new PlaceSphereState(this, world));
            tools.Add(KeyCode.Alpha2, new PlaceSphereStateMidair(this, world));

            activeState = tools[KeyCode.Alpha1];

            activeState.Start();

            raycaster = new VoxelWorldRaycaster();

        }

        private IState activeState;
        private Dictionary<KeyCode, IState> tools = new Dictionary<KeyCode, IState>();
        public void Update()
        {
            if (Input.mouseScrollDelta.y != 0)
                ActiveSize = changeSize(ActiveSize, Mathf.Sign(Input.mouseScrollDelta.y));

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                ActiveMaterial = changeMaterial(1);
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                ActiveMaterial = changeMaterial(-1);



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

        private float changeSize(float currentSize, float v)
        {
            return Mathf.Clamp(currentSize + v, 1, 100);
        }

        private VoxelMaterial changeMaterial(int indexChange)
        {
            return VoxelMaterials[(VoxelMaterials.IndexOf(ActiveMaterial) + indexChange + VoxelMaterials.Count) % VoxelMaterials.Count];
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

        private WorldEditTool tool = new WorldEditTool();

        private VoxelWorldScript voxelWorldScript;

        public PlaceSphereState(VoxelWorldScript voxelWorldScript, VoxelWorld world)
        {
            this.voxelWorldScript = voxelWorldScript;
            this.world = world;
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;

            var range = voxelWorldScript.ActiveSize;
            var material = voxelWorldScript.ActiveMaterial;

            var point = raycast.Value.point;
            tryPlaceSphere(range, material, point);

        }

        public void tryPlaceSphere(float range, VoxelMaterial material, Vector3 point)
        {
            var min = (point - new Vector3(1, 1, 1) * range).ToFloored();
            var max = (point + new Vector3(1, 1, 1) * range).ToCeiled();

            if (Input.GetMouseButtonDown(0))
            {
                world.ForChunksInRange(min, max, (p, c) =>
                {
                    var offset = p.Multiply(world.ChunkSize);
                    var localHit = point - offset;

                    tool.addSphere(c, localHit, range, material);
                    c.LastChangeFrame = Time.frameCount;

                });


            }
            else if (Input.GetMouseButtonDown(1))
            {
                world.ForChunksInRange(min, max, (p, c) =>
                {
                    var offset = p.Multiply(world.ChunkSize);
                    var localHit = point - offset;

                    tool.removeSphere(c, localHit, range, material);
                    c.LastChangeFrame = Time.frameCount;

                });
            }
        }

        public void Start()
        {
        }
    }

    public class PlaceSphereStateMidair : IState
    {
        private VoxelWorldScript script;
        private PlaceSphereState tool;
        private VoxelWorld world;

        public PlaceSphereStateMidair(VoxelWorldScript script, VoxelWorld world)
        {
            this.script = script;
            this.world = world;
            this.tool = new PlaceSphereState(script, world);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            var point = ray.origin + ray.direction * script.ActiveSize * 2f;

            tool.tryPlaceSphere(script.ActiveSize, script.ActiveMaterial, point);
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
