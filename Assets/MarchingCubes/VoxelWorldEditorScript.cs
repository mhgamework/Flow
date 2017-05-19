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
            tools.Add(KeyCode.Alpha2, new PlaceSphereStateMidair(this, world, SphereGizmo));
            tools.Add(KeyCode.Alpha3, new ReplaceMaterialTool(this, world, SphereGizmo));
            tools.Add(KeyCode.Alpha4, new FlattenTool(this, world, SphereGizmo, PlaneGizmo));
            tools.Add(KeyCode.Alpha5, new DrawOnPlaneTool(this, world, SphereGizmo, PlaneGizmo));
            tools.Add(KeyCode.Alpha6, new SmoothTool(this, world, SphereGizmo));

            activeState = tools[KeyCode.Alpha1];

            activeState.Start();

            raycaster = new VoxelWorldRaycaster();
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

            activeState.Update(raycaster.raycast());
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
