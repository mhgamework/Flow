using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DualContouring;
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
        public GameObject PlaneGizmo;
        private VoxelWorldRaycaster raycaster;

        public IState activeState;
        public Dictionary<KeyCode, IState> tools = new Dictionary<KeyCode, IState>();
        private List<VoxelMaterial> VoxelMaterials;

        public DrawWardTool DrawWardTool;
        public bool AddDefaultTools = true;


        private IEditableVoxelWorld world;

        public VoxelMaterial ActiveMaterial;
        public float ActiveSize = 3;

        private Dictionary<KeyCode,CreateTool> toolConstructors = new Dictionary<KeyCode,CreateTool>();
        public KeyCode StartTool = KeyCode.Alpha0;

        public void Start()
        {
            if (AddDefaultTools)
            {
                RegisterTool(KeyCode.Alpha0, new NullState());
                RegisterTool(KeyCode.Alpha1, new DepositTool(this, world, SphereGizmo));
                RegisterTool(KeyCode.Alpha2, new SmoothTool(this, world, SphereGizmo));
                //RegisterTool(KeyCode.Alpha3, DrawWardTool);
                RegisterTool(KeyCode.Alpha3, new FlattenTool(this, world, SphereGizmo, PlaneGizmo));
                RegisterTool(KeyCode.Alpha4, new ReplaceMaterialTool(this, world, SphereGizmo));
                RegisterTool(KeyCode.Alpha5, new PlaceSphereStateMidair(this, world, SphereGizmo));
                //RegisterTool(KeyCode.Alpha7, new PlaceSphereState(this, world, SphereGizmo));
                //RegisterTool(KeyCode.Alpha8, new DrawOnPlaneTool(this, world, SphereGizmo, PlaneGizmo));
            }
        }

        public void Init(IEditableVoxelWorld world, List<VoxelMaterial> voxelMaterials)
        {
            this.world = world;
            this.VoxelMaterials = voxelMaterials;

            ActiveMaterial = VoxelMaterials[0];

            DrawWardTool.Init(this, world, SphereGizmo);

            // Create and add all tools
            ActivateNewTools();
         
            activeState = tools[StartTool];
            activeState.Start();
            raycaster = new VoxelWorldRaycaster();
        }

        public delegate IState CreateTool(IEditableVoxelWorld world, GameObject sphereGizmo, GameObject planeGizmo);
        public void RegisterTool(KeyCode keycode, CreateTool ctr )
        {
            toolConstructors.Add(keycode, ctr);
        }
        private void RegisterTool(KeyCode keycode, IState tool)
        {
            RegisterTool(keycode, (a, b, c) => tool);
        }
        /// <summary>
        /// For dynamically adding tools at runtime
        /// </summary>
        public void ActivateNewTools()
        {
            foreach (var t in toolConstructors)
            {
                tools.Add(t.Key, t.Value(world, SphereGizmo, PlaneGizmo));
            }
            toolConstructors.Clear();
        }

        public void Update()
        {
            if (this.world == null) return;

            //addMovingSphere();


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

            //VR: activeState.Update(raycaster.raycast());
        }

        private void addMovingSphere()
        {
            var range = ActiveSize;
            var pos = 8 + 16 * 5;
            var point = new Vector3(pos + Mathf.Sin(Time.realtimeSinceStartup) * range,
                pos + Mathf.Cos(Time.realtimeSinceStartup) * range, pos);
            var material = ActiveMaterial;
            var radius = new Point3(1, 1, 1) * (int) Math.Ceiling(ActiveSize);

            world.RunKernel1by1(point.ToFloored() - radius, point.ToCeiled() + radius, (data, p) =>
            {
                data.Material = material;
                data.Density = (p - point).magnitude - range;
                return data;
            }, Time.frameCount);
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
}
