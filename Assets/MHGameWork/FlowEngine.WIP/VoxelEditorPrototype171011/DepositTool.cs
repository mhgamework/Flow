using System;
using Assets.MarchingCubes.Domain;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.VR;
using DirectX11;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class DepositTool : IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;
        private Vector3 point;

        public string Name
        {
            get { return "Deposit"; }
        }

        public DepositTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            //this.tool = new PlaceSphereState(script, world);
        }

        public void Start()
        {
            sphereGizmo.SetActive(true);

        }

        public void Stop()
        {
            sphereGizmo.SetActive(false);
        }

        public void Update(RaycastHit? raycast)
        {
            var pressed0 = Input.GetMouseButton(0);
            var pressed1 = Input.GetMouseButton(1);

            Update(raycast, pressed0, pressed1);
        }

        public void Update(RaycastHit? raycast, bool pressed0, bool pressed1)
        {
            if (!raycast.HasValue) return;

            point = raycast.Value.point;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize * VRSettings.RenderScale; //VR
            var range = script.ActiveSize;
            var material = script.ActiveMaterial;

            if (pressed0) DepositTerrainMaterial(range, material);
            if (pressed1) WithdrawTerrainMaterial(range, material);
        }

        protected virtual void DepositTerrainMaterial(float radius, VoxelMaterial material)
        {
            WorldEditOperations.DepositOrWithdrawTerrainMaterialImproved(new Point3(1, 1, 1) * (int)Math.Ceiling(radius), 0, material, point/ VRSettings.RenderScale, world, Time.frameCount);
        }

        protected virtual void WithdrawTerrainMaterial(float radius, VoxelMaterial material)
        {
            WorldEditOperations.DepositOrWithdrawTerrainMaterialImproved(new Point3(1, 1, 1) * (int)Math.Ceiling(radius), 1, material, point/ VRSettings.RenderScale, world, Time.frameCount);
        }




        public void OnDrawGizmos()
        {
        }
    }
}