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
    /// Contains logic for converting user input to voxel world edits
    /// Also holds the state for the editor
    /// </summary>
    public class VoxelWorldEditorScript : MonoBehaviour
    {
        public GameObject SphereGizmo;
        private VoxelWorldRaycaster raycaster;

        public IState activeState;
        public Dictionary<KeyCode, IState> tools = new Dictionary<KeyCode, IState>();
        private List<VoxelMaterial> VoxelMaterials;



        private IEditableVoxelWorld world;

        public VoxelMaterial ActiveMaterial;
        public float ActiveSize = 3;

        public void Start()
        {
        }

        public void Init(IEditableVoxelWorld world, List<VoxelMaterial> voxelMaterials)
        {
            this.world = world;
            this.VoxelMaterials = voxelMaterials;

            ActiveMaterial = VoxelMaterials[0];

            tools.Add(KeyCode.Alpha0, new NullState());
            tools.Add(KeyCode.Alpha1, new PlaceSphereState(this, world, SphereGizmo));
            tools.Add(KeyCode.Alpha2, new PlaceSphereStateMidair(this, world,SphereGizmo));
            tools.Add(KeyCode.Alpha3, new ReplaceMaterialTool(this, world,SphereGizmo));

            activeState = tools[KeyCode.Alpha1];

            activeState.Start();

            raycaster = new VoxelWorldRaycaster();
        }



        public void Update()
        {
            if (this.world == null) return;

            if (Input.mouseScrollDelta.y != 0)
                ActiveSize = changeSize(ActiveSize, Mathf.Sign(Input.mouseScrollDelta.y));

            if (Input.GetKeyDown(KeyCode.RightArrow))
                ActiveSize = changeSize(ActiveSize, 1);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                ActiveSize = changeSize(ActiveSize, -1);

            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.UpArrow))
                ActiveMaterial = changeMaterial(1);
            if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.DownArrow))
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

        public void OnDrawGizmos()
        {
            if (activeState != null)
                activeState.OnDrawGizmos();
        }
    }

    public interface IState
    {
        void Stop();
        void Update(RaycastHit? raycast);
        void Start();
        string Name { get; }
        void OnDrawGizmos();
    }

    class PlaceSphereState : IState
    {
        private readonly IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;

        private WorldEditTool tool = new WorldEditTool();

        private VoxelWorldEditorScript voxelWorldScript;
        private Vector3 point;

        public string Name
        {
            get { return "Add/Remove sphere"; }
        }

        public PlaceSphereState(VoxelWorldEditorScript voxelWorldScript, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.voxelWorldScript = voxelWorldScript;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;

            var range = voxelWorldScript.ActiveSize;
            var material = voxelWorldScript.ActiveMaterial;

            point = raycast.Value.point;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * range;

            tryPlaceSphere(range, material, point);

        }

        public void tryPlaceSphere(float range, VoxelMaterial material, Vector3 point)
        {
            var min = (point - new Vector3(1, 1, 1) * range).ToFloored();
            var max = (point + new Vector3(1, 1, 1) * range).ToCeiled();
            var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(range);

            if (Input.GetMouseButtonDown(0))
            {
                world.RunKernel1by1(point.ToFloored() - radius, point.ToCeiled() + radius, WorldEditTool.createAddSphereKernel(point, range, material), Time.frameCount);

                //world.ForChunksInRange(min, max, (p, c) =>
                //{
                //    var offset = p.Multiply(world.ChunkSize);
                //    var localHit = point - offset;

                //    tool.addSphere(c, localHit, range, material);
                //    c.LastChangeFrame = Time.frameCount;

                //});


            }
            else if (Input.GetMouseButtonDown(1))
            {
                world.RunKernel1by1(point.ToFloored() - radius, point.ToCeiled() + radius, WorldEditTool.createRemoveSphereKernel(point, range), Time.frameCount);

                //world.ForChunksInRange(min, max, (p, c) =>
                //{
                //    var offset = p.Multiply(world.ChunkSize);
                //    var localHit = point - offset;

                //    tool.removeSphere(c, localHit, range, material);
                //    c.LastChangeFrame = Time.frameCount;

                //});
            }
        }

        public void Start()
        {
        }

        public void OnDrawGizmos()
        {
        }
    }

    public class PlaceSphereStateMidair : IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;
        private Vector3 point;

        public string Name
        {
            get { return "Add/Remove sphere midair"; }
        }
        public PlaceSphereStateMidair(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            this.tool = new PlaceSphereState(script, world,sphereGizmo);
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

            point = ray.origin + ray.direction * script.ActiveSize * 2f;
            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;
            tool.tryPlaceSphere(script.ActiveSize, script.ActiveMaterial, point);
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(point, script.ActiveSize);

        }
    }

    public class ReplaceMaterialTool : IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;
        private Vector3 point;

        public string Name
        {
            get { return "Change Material"; }
        }
        public ReplaceMaterialTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            //this.tool = new PlaceSphereState(script, world);
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

            point = ray.origin + ray.direction * script.ActiveSize * 2f;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;

            tool.tryPlaceSphere(script.ActiveSize, script.ActiveMaterial, point);

        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(point, script.ActiveSize);
        }
    }

    class NullState : IState
    {
        public string Name
        {
            get { return "No tool"; }
        }
        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
        }

        public void Start()
        {
        }

        public void OnDrawGizmos()
        {

        }
    }
}
